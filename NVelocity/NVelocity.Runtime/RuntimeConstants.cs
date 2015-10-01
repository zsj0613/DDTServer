using System;
using System.Runtime.InteropServices;

namespace NVelocity.Runtime
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct RuntimeConstants
	{
		public const string RUNTIME_LOG = "runtime.log";

		public const string RUNTIME_LOG_LOGSYSTEM = "runtime.log.logsystem";

		public const string RUNTIME_LOG_LOGSYSTEM_CLASS = "runtime.log.logsystem.class";

		public const string RUNTIME_LOG_ERROR_STACKTRACE = "runtime.log.error.stacktrace";

		public const string RUNTIME_LOG_WARN_STACKTRACE = "runtime.log.warn.stacktrace";

		public const string RUNTIME_LOG_INFO_STACKTRACE = "runtime.log.info.stacktrace";

		public const string RUNTIME_LOG_REFERENCE_LOG_INVALID = "runtime.log.invalid.references";

		public const string DEBUG_PREFIX = " [debug] ";

		public const string INFO_PREFIX = "  [info] ";

		public const string WARN_PREFIX = "  [warn] ";

		public const string ERROR_PREFIX = " [error] ";

		public const string UNKNOWN_PREFIX = " [unknown] ";

		public const string COUNTER_NAME = "directive.foreach.counter.name";

		public const string COUNTER_INITIAL_VALUE = "directive.foreach.counter.initial.value";

		public const string ERRORMSG_START = "directive.include.output.errormsg.start";

		public const string ERRORMSG_END = "directive.include.output.errormsg.end";

		public const string PARSE_DIRECTIVE_MAXDEPTH = "directive.parse.max.depth";

		public const string RESOURCE_MANAGER_CLASS = "resource.manager.class";

		public const string RESOURCE_MANAGER_CACHE_CLASS = "resource.manager.cache.class";

		public const string RESOURCE_MANAGER_DEFAULTCACHE_SIZE = "resource.manager.defaultcache.size";

		public const string RESOURCE_MANAGER_LOGWHENFOUND = "resource.manager.logwhenfound";

		public const string RESOURCE_LOADER = "resource.loader";

		public const string FILE_RESOURCE_LOADER_PATH = "file.resource.loader.path";

		public const string FILE_RESOURCE_LOADER_CACHE = "file.resource.loader.cache";

		public const string VM_LIBRARY = "velocimacro.library";

		public const string VM_LIBRARY_AUTORELOAD = "velocimacro.library.autoreload";

		public const string VM_PERM_ALLOW_INLINE = "velocimacro.permissions.allow.inline";

		public const string VM_PERM_ALLOW_INLINE_REPLACE_GLOBAL = "velocimacro.permissions.allow.inline.to.replace.global";

		public const string VM_PERM_INLINE_LOCAL = "velocimacro.permissions.allow.inline.local.scope";

		public const string VM_MESSAGES_ON = "velocimacro.messages.on";

		public const string VM_CONTEXT_LOCALSCOPE = "velocimacro.context.localscope";

		public const string INTERPOLATE_STRINGLITERALS = "runtime.interpolate.string.literals";

		public const string INPUT_ENCODING = "input.encoding";

		public const string OUTPUT_ENCODING = "output.encoding";

		public const string ENCODING_DEFAULT = "ISO-8859-1";

		public const string DEFAULT_RUNTIME_PROPERTIES = "NVelocity.Runtime.Defaults.nvelocity.properties";

		public const string DEFAULT_RUNTIME_DIRECTIVES = "NVelocity.Runtime.Defaults.directive.properties";

		public const int NUMBER_OF_PARSERS = 20;

		public const string PARSER_POOL_SIZE = "parser.pool.size";

		public const string UBERSPECT_CLASSNAME = "runtime.introspector.uberspect";
	}
}
