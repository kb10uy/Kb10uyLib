using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using Kb10uy.Extension;

namespace Kb10uy.IO
{

    /// <summary>
    /// 構造体などをバイナリデータとしてシリアライズ・デシリアライズする機能を提供します。
    /// </summary>
    /// <typeparam name="T">型</typeparam>
    public class BinaryContractSerializer<T>
    {

        /// <summary>
        /// 現在の型のTypeオブジェクト。
        /// </summary>
        public Type TypeObject { get; set; }

        /// <summary>
        /// BinaryContractMember属性が適用されているフィールド。
        /// </summary>
        public IList<FieldInfo> AllowedFields { get; protected set; }

        /// <summary>
        /// BinaryContractMember属性が適用されているプロパティ。
        /// </summary>
        public IList<PropertyInfo> AllowedProperties { get; protected set; }

        /// <summary>
        /// 型情報を解析し、シリアライズの初期化をします。
        /// </summary>
        public BinaryContractSerializer()
        {
            TypeObject = typeof(T);
            if (TypeObject.GetCustomAttribute<BinaryContractAttribute>() == null)
            {
                throw new NotSupportedException("BinaryContract属性が指定されていない型です : " + TypeObject.Name);
            }
            AllowedFields = TypeObject.GetFields()
                .Where((p) => p.GetCustomAttribute<BinaryContractMemberAttribute>() != null)
                .ToList();
            AllowedProperties = TypeObject.GetProperties()
                .Where((p) => p.GetCustomAttribute<BinaryContractMemberAttribute>() != null)
                .ToList();
        }

        /*
         * 型格納はフィールド、プロパティの準でやる。
         */

        /// <summary>
        /// オブジェクトをバイト列に変換します。
        /// </summary>
        /// <param name="obj">オブジェクト</param>
        /// <returns>バイト列</returns>
        public byte[] GetByteArray(T obj)
        {
            var ba = new List<byte>();
            //フィールド
            foreach (var f in AllowedFields)
            {
                var cv = f.FieldType.GetByteEncodeFunction();
                if (cv == null)
                {
                    var atr = f.GetCustomAttribute<BinaryContractMemberAttribute>();
                    if (atr.EncodeFunction == null)
                    {
                        throw new NotSupportedException(
                            String.Format("変換できない型です。基本型にするか、EncodeFunctionを指定してください : {0}.{1} ({2}型)",
                            TypeObject.Name,
                            f.Name,
                            f.FieldType.Name
                            )
                        );
                    }
                    else
                    {
                        ba.AddRange(atr.EncodeFunction(f.GetValue(obj)));
                    }
                }
                else
                {
                    ba.AddRange(cv(f.GetValue(obj)));
                }
            }
            //プロパティ
            foreach (var p in AllowedProperties)
            {
                var cv = p.PropertyType.GetByteEncodeFunction();
                if (cv == null)
                {
                    var atr = p.GetCustomAttribute<BinaryContractMemberAttribute>();
                    if (atr.EncodeFunction == null)
                    {
                        throw new NotSupportedException(
                            String.Format("変換できない型です。基本型にするか、EncodeFunctionを指定してください : {0}.{1} ({2}型)",
                            TypeObject.Name,
                            p.Name,
                            p.PropertyType.Name
                            )
                        );
                    }
                    else
                    {
                        ba.AddRange(atr.EncodeFunction(p.GetValue(obj)));
                    }
                }
                else
                {
                    ba.AddRange(cv(p.GetValue(obj)));
                }
            }
            return ba.ToArray();
        }

    }

    /// <summary>
    /// BinaryContractSerializerを使用するクラス・構造体に適用する属性。
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Class
        | AttributeTargets.Struct,
        Inherited = false,
        AllowMultiple = false
    )]
    public sealed class BinaryContractAttribute : Attribute
    {

    }

    /// <summary>
    /// BinaryContractSerializerで使用するメンバに適用する属性。
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Field
        | AttributeTargets.Property,
        Inherited = false,
        AllowMultiple = false
    )]
    public sealed class BinaryContractMemberAttribute : Attribute
    {
        /// <summary>
        /// 値がdefault(T)と同一だった場合に指定する値の文字列。
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// 値をバイト列に変換する際に使用する関数。
        /// </summary>
        public Func<object, byte[]> EncodeFunction { get; set; }

        /// <summary>
        /// 値をバイト列から元に戻す祭に使用する関数。
        /// </summary>
        public Func<byte[], object> DecodeFunction { get; set; }
    }

}
