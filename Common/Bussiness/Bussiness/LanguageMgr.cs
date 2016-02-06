using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bussiness
{
    public class LanguageMgr
    {
        public static bool Load() => Game.Language.LanguageMgr.Load();
        public static string GetTranslation(string translateId, params object[] args) => Game.Language.LanguageMgr.GetTranslation(translateId, args);
    }
}
