
using Lsj.Util.Logs;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
namespace Game.Base.Managers
{
	public class ScriptMgr
	{
        static LogProvider log => LogProvider.Default;
        private static readonly Dictionary<string, Assembly> m_scripts = new Dictionary<string, Assembly>();
		public static Assembly[] Scripts
		{
			get
			{
				Dictionary<string, Assembly> scripts;
				Monitor.Enter(scripts = ScriptMgr.m_scripts);
				Assembly[] result;
				try
				{
					result = ScriptMgr.m_scripts.Values.ToArray<Assembly>();
				}
				finally
				{
					Monitor.Exit(scripts);
				}
				return result;
			}
		}
		public static bool InsertAssembly(Assembly ass)
		{
			Dictionary<string, Assembly> scripts;
			Monitor.Enter(scripts = ScriptMgr.m_scripts);
			bool result;
			try
			{
                ScriptMgr.log.Debug(ass.FullName);
                if (!ScriptMgr.m_scripts.ContainsKey(ass.FullName))
				{
					ScriptMgr.m_scripts.Add(ass.FullName, ass);
                    
					result = true;
				}
				else
				{
					result = false;
				}
			}
			finally
			{
				Monitor.Exit(scripts);
			}
			return result;
		}
		public static bool RemoveAssembly(Assembly ass)
		{
			Dictionary<string, Assembly> scripts;
			Monitor.Enter(scripts = ScriptMgr.m_scripts);
			bool result;
			try
			{
				result = ScriptMgr.m_scripts.Remove(ass.FullName);
			}
			finally
			{
				Monitor.Exit(scripts);
			}
			return result;
		}
		public static bool CompileScripts(bool compileVB, string path, string dllName, string[] asm_names)
		{
			if (!path.EndsWith("\\") && !path.EndsWith("/"))
			{
				path += "/";
			}
			ArrayList files = ScriptMgr.ParseDirectory(new DirectoryInfo(path), compileVB ? "*.vb" : "*.cs", true);
			bool result;
			if (files.Count == 0)
			{
				result = true;
			}
			else
			{
				if (File.Exists(dllName))
				{
					File.Delete(dllName);
				}
				CompilerResults res = null;
				try
				{
					CodeDomProvider compiler;
					if (compileVB)
					{
						compiler = new VBCodeProvider();
					}
					else
					{
						compiler = new CSharpCodeProvider();
					}
					CompilerParameters param = new CompilerParameters(asm_names, dllName, true);
					param.GenerateExecutable = false;
					param.GenerateInMemory = false;
					param.WarningLevel = 2;
					param.CompilerOptions = "/lib:.";
					string[] filepaths = new string[files.Count];
					for (int i = 0; i < files.Count; i++)
					{
						filepaths[i] = ((FileInfo)files[i]).FullName;
					}
					res = compiler.CompileAssemblyFromFile(param, filepaths);
					GC.Collect();
					if (res.Errors.HasErrors)
					{
						foreach (CompilerError err in res.Errors)
						{
							if (!err.IsWarning)
							{
								StringBuilder builder = new StringBuilder();
								builder.Append("   ");
								builder.Append(err.FileName);
								builder.Append(" Line:");
								builder.Append(err.Line);
								builder.Append(" Col:");
								builder.Append(err.Column);
								ScriptMgr.log.Error("Script compilation failed because: ");
									ScriptMgr.log.Error(err.ErrorText);
									ScriptMgr.log.Error(builder.ToString());
								
							}
						}
						result = false;
						return result;
					}
				}
				catch (Exception e)
				{
					
						ScriptMgr.log.Error("CompileScripts", e);
					
				}
				if (res != null && !res.Errors.HasErrors)
				{
					ScriptMgr.InsertAssembly(res.CompiledAssembly);
				}
				result = true;
			}
			return result;
		}
		private static ArrayList ParseDirectory(DirectoryInfo path, string filter, bool deep)
		{
			ArrayList files = new ArrayList();
			ArrayList result;
			if (!path.Exists)
			{
				result = files;
			}
			else
			{
				files.AddRange(path.GetFiles(filter));
				if (deep)
				{
					DirectoryInfo[] directories = path.GetDirectories();
					for (int i = 0; i < directories.Length; i++)
					{
						DirectoryInfo subdir = directories[i];
						files.AddRange(ScriptMgr.ParseDirectory(subdir, filter, deep));
					}
				}
				result = files;
			}
			return result;
		}
		public static Type GetType(string name)
		{
			Assembly[] scripts = ScriptMgr.Scripts;
			Type result;
			for (int i = 0; i < scripts.Length; i++)
			{
				Assembly asm = scripts[i];
				Type t = asm.GetType(name);
				if (t != null)
				{
					result = t;
					return result;
				}
			}
			result = null;
			return result;
		}
		public static object CreateInstance(string name)
		{
			Assembly[] scripts = ScriptMgr.Scripts;
			object result;
			for (int i = 0; i < scripts.Length; i++)
			{
				Assembly asm = scripts[i];
				Type t = asm.GetType(name);
				if (t != null && t.IsClass)
				{
					result = Activator.CreateInstance(t);
					return result;
				}
			}
			result = null;
			return result;
		}
		public static object CreateInstance(string name, Type baseType)
		{
			Assembly[] scripts = ScriptMgr.Scripts;
			object result;
			for (int i = 0; i < scripts.Length; i++)
			{
				Assembly asm = scripts[i];
				Type t = asm.GetType(name);
				if (t != null && t.IsClass && baseType.IsAssignableFrom(t))
				{
					result = Activator.CreateInstance(t);
					return result;
				}
			}
			result = null;
			return result;
		}
		public static Type[] GetDerivedClasses(Type baseType)
		{
			Type[] result;
			if (baseType == null)
			{
				result = new Type[0];
			}
			else
			{
				ArrayList types = new ArrayList();
				ArrayList asms = new ArrayList(ScriptMgr.Scripts);
				foreach (Assembly asm in asms)
				{
					Type[] types2 = asm.GetTypes();
					for (int i = 0; i < types2.Length; i++)
					{
						Type t = types2[i];
						if (t.IsClass && baseType.IsAssignableFrom(t))
						{
							types.Add(t);
						}
					}
				}
				result = (Type[])types.ToArray(typeof(Type));
			}
			return result;
		}
		public static Type[] GetImplementedClasses(string baseInterface)
		{
			ArrayList types = new ArrayList();
			ArrayList asms = new ArrayList(ScriptMgr.Scripts);
			foreach (Assembly asm in asms)
			{
				Type[] types2 = asm.GetTypes();
				for (int i = 0; i < types2.Length; i++)
				{
					Type t = types2[i];
					if (t.IsClass && t.GetInterface(baseInterface) != null)
					{
						types.Add(t);
					}
				}
			}
			return (Type[])types.ToArray(typeof(Type));
		}
	}
}
