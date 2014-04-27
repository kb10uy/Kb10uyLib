using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kb10uy.Extension
{

    /// <summary>
    /// 数学的な拡張機能を提供します。
    /// </summary>
    public static class MathExtension
    {
        /// <summary>
        /// 最大公約数を求めます。
        /// </summary>
        /// <returns></returns>
        public static int GetGCD(int a, int b)
        {
            if (a < b) GeneralExtension.Swap(ref a, ref b);
            var r = a % b;
            if (r == 0) return r;
            else return GetGCD(b, r);
        }

        /// <summary>
        /// 数値の十進数における桁数を求めます。
        /// </summary>
        /// <param name="n">数値</param>
        /// <returns>桁数</returns>
        public static int GetDigitCount(this int n)
        {
            return (int)(Math.Log10(n)) + 1;
        }

        /// <summary>
        /// 数値の任意の基数における桁数を求めます。
        /// </summary>
        /// <param name="n">数値</param>
        /// <param name="b">基数</param>
        /// <returns>桁数</returns>
        public static int GetDigitCount(this int n, int b)
        {
            return (int)(Math.Log(n) / Math.Log(b)) + 1;
        }

    }

    /// <summary>
    /// 汎用的な拡張機能を提供します。
    /// </summary>
    public static class GeneralExtension
    {
        /// <summary>
        /// スワップします。
        /// </summary>
        /// <typeparam name="T">交換したい値の型</typeparam>
        /// <param name="p1">値</param>
        /// <param name="p2">値</param>
        public static void Swap<T>(ref T p1, ref T p2)
        {
            T b = p1;
            p1 = p2;
            p2 = p1;
        }

        /// <summary>
        /// List&lt;string&gt;にString.Format()を使って追加するのがめんどくさいという人に
        /// </summary>
        /// <param name="s">本体</param>
        /// <param name="fmt">フォーマット文字列</param>
        /// <param name="obj">paramsに指定する奴</param>
        public static void Add(this IList<string> s, string fmt, params object[] obj)
        {
            s.Add(String.Format(fmt, obj));
        }

        /// <summary>
        /// 文字列をバイト列に変換します。誰得関数。
        /// </summary>
        /// <param name="s">文字列</param>
        /// <returns>バイト列</returns>
        public static byte[] ToByteArray(this string s)
        {
            var t = new List<byte>();
            foreach (var c in s == null ? "" : s)
            {
                t.AddRange(BitConverter.GetBytes(c));
            }
            return t.ToArray();
        }

        /// <summary>
        /// 指定の文字が最初に来るまでの文字列を取得します。
        /// </summary>
        /// <param name="str">文字列</param>
        /// <param name="c">文字</param>
        /// <returns>そこまでの文字</returns>
        public static string SubstringTo(this string str, char c)
        {
            return str.Substring(0, str.IndexOf(c) == -1 ? 0 : str.IndexOf(c));
        }

        /// <summary>
        /// 文字列を繰り返します。
        /// </summary>
        /// <param name="s">文字列</param>
        /// <param name="n">回数</param>
        /// <returns>くりかえされた</returns>
        public static string Repeat(this string s, int n)
        {
            var r = "";
            for (int i = 0; i < n; i++)
            {
                r += s;
            }
            return r;
        }

        /// <summary>
        /// シーケンスを指定回数繰り返します。
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="ie">繰り返すシーケンス</param>
        /// <param name="c">回数</param>
        /// <returns>繰り返されたシーケンス</returns>
        public static IEnumerable<T> Times<T>(this IEnumerable<T> ie, int c)
        {
            for (int i = 0; i < c; i++)
            {
                foreach (var e in ie)
                {
                    yield return e;
                }
            }
        }

        /// <summary>
        /// シーケンスを指定された個数ごとに分け、それぞれの最初の要素を新しいシーケンスとして返します。
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="ie">処理するシーケンス</param>
        /// <param name="c">分ける個数</param>
        /// <returns></returns>
        public static IEnumerable<T> TakeFirstEvery<T>(this IEnumerable<T> ie, int c)
        {
            int j = 0;
            foreach (var i in ie)
            {
                if (j == 0) yield return i;
                j++;
                if (j == c) j = 0;
            }
        }

        /// <summary>
        /// 現在のIDictionaryオブジェクトを指定のキーで検索し、
        /// 見つかった場合にはそのキーの値を、
        /// 見つからなかった場合はそのキーで新しい値を作成し、
        /// それぞれ値を返却します。
        /// <para>キーが存在したか確認したい場合は、TryGetValueメソッドを使用してください。</para>
        /// <remarks>新しい値には、デフォルトの引数無しコンストラクタが使用されます。</remarks>
        /// </summary>
        /// <typeparam name="TKey">IDictionaryのTKey</typeparam>
        /// <typeparam name="TValue">IDictionaryのTKey</typeparam>
        /// <param name="dic">IDictionarオブジェクトy</param>
        /// <param name="key">対象のキー</param>
        /// <returns></returns>
        public static TValue GetOrCreateValue<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key)
        where TValue : new()
        {
            if (!dic.ContainsKey(key))
            {
                dic[key] = new TValue();
            }
            
            return dic[key];
        }

    }

    /// <summary>
    /// リフレクション関係の拡張メソッドを提供。
    /// </summary>
    public static class ReflectionExtension
    {
        /// <summary>
        /// BitConverterクラスから適当に取ってきて使いやすいかたちに変えておきます
        /// </summary>
        /// <param name="t">Typeオブジェクト</param>
        /// <returns>該当する型なら</returns>
        public static Func<object, byte[]> GetByteEncodeFunction(this Type t)
        {
            switch (t.Name)
            {
                case "Int32":
                    return (object p) => BitConverter.GetBytes((int)p);
                case "UInt32":
                    return (object p) => BitConverter.GetBytes((uint)p);
                case "Int64":
                    return (object p) => BitConverter.GetBytes((long)p);
                case "UInt64":
                    return (object p) => BitConverter.GetBytes((ulong)p);
                case "Int16":
                    return (object p) => BitConverter.GetBytes((short)p);
                case "UInt16":
                    return (object p) => BitConverter.GetBytes((ushort)p);
                case "Byte":
                    return (object p) => BitConverter.GetBytes((byte)p);
                case "SByte":
                    return (object p) => BitConverter.GetBytes((sbyte)p);
                case "Double":
                    return (object p) => BitConverter.GetBytes((double)p);
                case "Single":
                    return (object p) => BitConverter.GetBytes((float)p);
                case "Char":
                    return (object p) => BitConverter.GetBytes((char)p);
                case "Boolean":
                    return (object p) => BitConverter.GetBytes((bool)p);
                case "String":
                    return (object p) => ((string)p).ToByteArray();
                default:
                    return null;
            }
        }
    }

}
