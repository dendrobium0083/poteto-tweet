using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Poteto.Application.DTOs;
using Poteto.Application.Interfaces.Repositories;
using Poteto.Application.Interfaces.Services;
using Poteto.Domain.Entities;

namespace Poteto.Application.Services
{
    public class BlockService : IBlockService
    {
        private readonly IBlockRepository _blockRepository;

        public BlockService(IBlockRepository blockRepository)
        {
            _blockRepository = blockRepository ?? throw new ArgumentNullException(nameof(blockRepository));
        }

        /// <summary>
        /// 新規ブロックを登録し、登録されたブロック情報の DTO を返します。
        /// </summary>
        public async Task<BlockDTO> CreateBlockAsync(int blockerId, int blockedId)
        {
            // ドメインエンティティの生成
            var block = new Block(blockerId, blockedId);

            // リポジトリ経由でブロックを登録し、自動採番された BlockId を取得
            int blockId = await _blockRepository.CreateBlockAsync(block);

            // 登録後のブロック情報を取得（または、生成したエンティティに blockId を設定してもよい）
            block = await _blockRepository.GetBlockAsync(blockerId, blockedId);
            if (block == null)
            {
                throw new InvalidOperationException("Block data could not be found after creation.");
            }

            // DTO への変換
            return new BlockDTO
            {
                Id = block.BlockId,
                BlockerId = block.BlockerId,
                BlockedId = block.BlockedId,
                CreatedAt = block.CreatedAt
            };
        }

        /// <summary>
        /// 指定したユーザ（ブロックを行ったユーザ）が行った全てのブロック一覧を取得します。
        /// </summary>
        public async Task<IEnumerable<BlockDTO>> GetBlocksByBlockerIdAsync(int blockerId)
        {
            var blocks = await _blockRepository.GetBlocksByBlockerIdAsync(blockerId);
            return blocks.Select(b => new BlockDTO
            {
                Id = b.BlockId,
                BlockerId = b.BlockerId,
                BlockedId = b.BlockedId,
                CreatedAt = b.CreatedAt
            });
        }

        /// <summary>
        /// 指定されたブロック関係を解除します。
        /// </summary>
        public async Task DeleteBlockAsync(int blockerId, int blockedId)
        {
            await _blockRepository.DeleteBlockAsync(blockerId, blockedId);
        }
    }
}
