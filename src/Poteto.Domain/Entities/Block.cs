using System;

namespace Poteto.Domain.Entities
{
    public class Block
    {
        // ブロック関係の一意な識別子
        public int BlockId { get; private set; }

        // ブロックを行うユーザのID
        public int BlockerId { get; private set; }

        // ブロック対象のユーザのID
        public int BlockedId { get; private set; }

        // ブロック登録日時（UTC）
        public DateTime CreatedAt { get; private set; }

        // コンストラクタ：新規ブロック関係の作成時に利用
        public Block(int blockerId, int blockedId)
        {
            if (blockerId <= 0)
                throw new ArgumentException("BlockerIdは正の値でなければなりません。", nameof(blockerId));
            if (blockedId <= 0)
                throw new ArgumentException("BlockedIdは正の値でなければなりません。", nameof(blockedId));
            if (blockerId == blockedId)
                throw new ArgumentException("自分自身をブロックすることはできません。", nameof(blockedId));

            BlockerId = blockerId;
            BlockedId = blockedId;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
