using Commons.Collections;
using NVelocity.Exception;
using NVelocity.Runtime.Resource.Loader;
using System;
using System.Collections;
using System.IO;

namespace NVelocity.Runtime.Resource
{
	public class ResourceManagerImpl : IResourceManager
	{
		private const string RESOURCE_LOADER_IDENTIFIER = "_RESOURCE_LOADER_IDENTIFIER_";

		protected internal ResourceCache globalCache = null;

		protected internal ArrayList resourceLoaders;

		private ArrayList sourceInitializerList;

		private bool resourceLoaderInitializersActive = false;

		private bool logWhenFound = true;

		protected internal IRuntimeServices rsvc = null;

		public ResourceManagerImpl()
		{
			this.resourceLoaders = new ArrayList();
			this.sourceInitializerList = new ArrayList();
		}

		public void Initialize(IRuntimeServices rs)
		{
			this.rsvc = rs;
			this.rsvc.Info("Default ResourceManager initializing. (" + base.GetType() + ")");
			this.AssembleResourceLoaderInitializers();
			for (int i = 0; i < this.sourceInitializerList.Count; i++)
			{
				ExtendedProperties extendedProperties = (ExtendedProperties)this.sourceInitializerList[i];
				string @string = extendedProperties.GetString("class");
				if (@string == null)
				{
					this.rsvc.Error("Unable to find '" + extendedProperties.GetString("_RESOURCE_LOADER_IDENTIFIER_") + ".resource.loader.class' specification in configuation. This is a critical value.  Please adjust configuration.");
				}
				else
				{
					ResourceLoader loader = ResourceLoaderFactory.getLoader(this.rsvc, @string);
					loader.CommonInit(this.rsvc, extendedProperties);
					loader.Init(extendedProperties);
					this.resourceLoaders.Add(loader);
				}
			}
			this.logWhenFound = this.rsvc.GetBoolean("resource.manager.logwhenfound", true);
			string string2 = this.rsvc.GetString("resource.manager.cache.class");
			object obj = null;
			if (string2 != null && string2.Length > 0)
			{
				try
				{
					Type type = Type.GetType(string2);
					obj = Activator.CreateInstance(type);
				}
				catch (System.Exception var_7_135)
				{
					string message = "The specified class for ResourceCache (" + string2 + ") does not exist (or is not accessible to the current classloader).";
					this.rsvc.Error(message);
					obj = null;
				}
				if (!(obj is ResourceCache))
				{
					string message = "The specified class for ResourceCache (" + string2 + ") does not implement NVelocity.Runtime.Resource.ResourceCache. Using default ResourceCache implementation.";
					this.rsvc.Error(message);
					obj = null;
				}
			}
			if (obj == null)
			{
				obj = new ResourceCacheImpl();
			}
			this.globalCache = (ResourceCache)obj;
			this.globalCache.initialize(this.rsvc);
			this.rsvc.Info("Default ResourceManager initialization complete.");
		}

		private void AssembleResourceLoaderInitializers()
		{
			if (!this.resourceLoaderInitializersActive)
			{
				ArrayList vector = this.rsvc.Configuration.GetVector("resource.loader");
				for (int i = 0; i < vector.Count; i++)
				{
					string prefix = vector[i] + ".resource.loader";
					ExtendedProperties extendedProperties = this.rsvc.Configuration.Subset(prefix);
					if (extendedProperties == null)
					{
						this.rsvc.Warn("ResourceManager : No configuration information for resource loader named '" + vector[i] + "'. Skipping.");
					}
					else
					{
						extendedProperties.SetProperty("_RESOURCE_LOADER_IDENTIFIER_", vector[i]);
						this.sourceInitializerList.Add(extendedProperties);
					}
				}
				this.resourceLoaderInitializersActive = true;
			}
		}

		public Resource GetResource(string resourceName, ResourceType resourceType, string encoding)
		{
			Resource resource = this.globalCache.get(resourceName);
			Resource result;
			if (resource != null)
			{
				try
				{
					this.RefreshResource(resource, encoding);
				}
				catch (ResourceNotFoundException var_1_26)
				{
					this.globalCache.remove(resourceName);
					result = this.GetResource(resourceName, resourceType, encoding);
					return result;
				}
				catch (ParseErrorException arg)
				{
					this.rsvc.Error("ResourceManager.GetResource() exception: " + arg);
					throw;
				}
				catch (System.Exception arg2)
				{
					this.rsvc.Error("ResourceManager.GetResource() exception: " + arg2);
					throw;
				}
			}
			else
			{
				try
				{
					resource = this.LoadResource(resourceName, resourceType, encoding);
					if (resource.ResourceLoader.CachingOn)
					{
						this.globalCache.put(resourceName, resource);
					}
				}
				catch (ResourceNotFoundException var_4_B5)
				{
					this.rsvc.Error("ResourceManager : unable to find resource '" + resourceName + "' in any resource loader.");
					throw;
				}
				catch (ParseErrorException arg)
				{
					this.rsvc.Error("ResourceManager.GetResource() parse exception: " + arg);
					throw;
				}
				catch (System.Exception arg3)
				{
					this.rsvc.Error("ResourceManager.GetResource() exception new: " + arg3);
					throw;
				}
			}
			result = resource;
			return result;
		}

		protected internal Resource LoadResource(string resourceName, ResourceType resourceType, string encoding)
		{
			Resource resource = ResourceFactory.GetResource(resourceName, resourceType);
			resource.RuntimeServices = this.rsvc;
			resource.Name = resourceName;
			resource.Encoding = encoding;
			long lastModified = 0L;
			ResourceLoader resourceLoader = null;
			for (int i = 0; i < this.resourceLoaders.Count; i++)
			{
				resourceLoader = (ResourceLoader)this.resourceLoaders[i];
				resource.ResourceLoader = resourceLoader;
				try
				{
					if (resource.Process())
					{
						if (this.logWhenFound)
						{
							this.rsvc.Info("ResourceManager : found " + resourceName + " with loader " + resourceLoader.ClassName);
						}
						lastModified = resourceLoader.GetLastModified(resource);
						break;
					}
				}
				catch (ResourceNotFoundException)
				{
				}
			}
			if (resource.Data == null)
			{
				throw new ResourceNotFoundException("Unable to find resource '" + resourceName + "'");
			}
			resource.LastModified = lastModified;
			resource.ModificationCheckInterval = resourceLoader.ModificationCheckInterval;
			resource.Touch();
			return resource;
		}

		protected internal void RefreshResource(Resource resource, string encoding)
		{
			if (resource.RequiresChecking())
			{
				resource.Touch();
				if (resource.IsSourceModified())
				{
					if (!resource.Encoding.Equals(encoding))
					{
						this.rsvc.Error(string.Concat(new string[]
						{
							"Declared encoding for template '",
							resource.Name,
							"' is different on reload.  Old = '",
							resource.Encoding,
							"'  New = '",
							encoding
						}));
						resource.Encoding = encoding;
					}
					long lastModified = resource.ResourceLoader.GetLastModified(resource);
					resource.Process();
					resource.LastModified = lastModified;
				}
			}
		}

		public Resource GetResource(string resourceName, ResourceType resourceType)
		{
			return this.GetResource(resourceName, resourceType, "ISO-8859-1");
		}

		public string GetLoaderNameForResource(string resourceName)
		{
			string result;
			for (int i = 0; i < this.resourceLoaders.Count; i++)
			{
				ResourceLoader resourceLoader = (ResourceLoader)this.resourceLoaders[i];
				Stream stream = null;
				try
				{
					stream = resourceLoader.GetResourceStream(resourceName);
					if (stream != null)
					{
						result = resourceLoader.GetType().ToString();
						return result;
					}
				}
				catch (ResourceNotFoundException var_3_42)
				{
				}
				finally
				{
					if (stream != null)
					{
						try
						{
							stream.Close();
						}
						catch (IOException var_4_61)
						{
						}
					}
				}
			}
			result = null;
			return result;
		}
	}
}
