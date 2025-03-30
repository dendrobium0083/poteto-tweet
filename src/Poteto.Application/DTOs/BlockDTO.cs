using System;

namespace Poteto.Application.DTOs
{
    /// <summary>
    /// ブロック情報を転送するためのデータ転送オブジェクト
    /// </summary>
    public class BlockDTO
    {
        /// <summary>
        /// ブロック関係の一意な識別子
        /// </summary>
        public int BlockId { get; set; }
        
        /// <summary>
        /// ブロックを行ったユーザのID
        /// </summary>
        public int BlockerId { get; set; }
        
        /// <summary>
        /// ブロック対象のユーザのID
        /// </summary>
        public int BlockedId { get; set; }
        
        /// <summary>
        /// ブロック登録日時（UTC）
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
