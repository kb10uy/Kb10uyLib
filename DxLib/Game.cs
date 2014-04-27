using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DxLibDLL;

namespace Kb10uy.DxLib
{
    /// <summary>
    /// XNAのような構造でDXライブラリの機能を提供します。
    /// </summary>
	public class Game
	{
		/// <summary>
		/// DXに入る前の初期化をします。
		/// </summary>
		public virtual void InitializeBeforeDxLib()
		{
			
		}
		/// <summary>
		/// DX初期化後の初期化をします。
		/// </summary>
        /// <returns>ここまでの初期化が成功したかのフラグ</returns>
		public virtual bool Initialize()
		{
            return true;
		}

		/// <summary>
		/// ゲーム内処理をします。
		/// </summary>
		/// <param name="frame">フレーム数</param>
		/// <returns>ゲームを終了するかのフラグ</returns>
		public virtual bool Tick(int frame)
		{
			return false;
		}

		/// <summary>
		/// 描画します。
		/// </summary>
		public virtual void Draw()
		{

		}

		/// <summary>
		/// リソースの開放をします。
		/// </summary>
		public virtual void Dispose()
		{

		}

	}
}
