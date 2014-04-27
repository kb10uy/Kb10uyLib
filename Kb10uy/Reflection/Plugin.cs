using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace Kb10uy.Reflection
{
    /// <summary>
    /// プラグイン機構の実装を安易にする機能を提供します。
    /// </summary>
    public class Plugin
    {

        /// <summary>
        /// 指定ファイルのアセンブリ内にある型を取得します。
        /// </summary>
        /// <param name="file">ファイル名</param>
        /// <returns>アセンブリ内にある型。ファイルが存在しなかった場合は例外。</returns>
        public IEnumerable<Type> GetAssemblyTypes(string file)
        {
            List<Type> list;

            try
            {
                Assembly asm = Assembly.LoadFile(file);
                list=asm.GetTypes().ToList();
            }
            catch (FileNotFoundException e)
            {
                throw;
            }
            return list;
        }


    }
}
