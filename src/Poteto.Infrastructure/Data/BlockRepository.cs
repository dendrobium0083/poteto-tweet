using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Poteto.Domain.Entities;

namespace Poteto.Infrastructure.Data
{
    // Block に対する DB 操作を抽象化するインターフェース
    public interface IBlockRepository
    {
        Task<Block> GetBlockAsync(int blockerId, int blockedId);
        Task<IEnumerable<Block>> GetBlocksByBlockerIdAsync(int blockerId);
        Task<int> CreateBlockAsync(Block block);
        Task DeleteBlockAsync(int blockerId, int blockedId);
    }

    // Dapper を利用した BlockRepository の実装
    public class BlockRepository : IBlockRepository
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        // IUnitOfWork から接続およびトランザクションを取得
        public BlockRepository(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            _connection = unitOfWork.Transaction.Connection;
            _transaction = unitOfWork.Transaction;
        }

        // 指定のブロック関係（Blocker と Blocked の組み合わせ）を取得
        public async Task<Block> GetBlockAsync(int blockerId, int blockedId)
        {
            string sql = "SELECT * FROM Blocks WHERE BlockerId = :BlockerId AND BlockedId = :BlockedId";
            return await _connection.QueryFirstOrDefaultAsync<Block>(
                sql,
                new { BlockerId = blockerId, BlockedId = blockedId },
                _transaction
            );
        }

        // 指定のユーザ（Blocker）が行った全てのブロックを取得
        public async Task<IEnumerable<Block>> GetBlocksByBlockerIdAsync(int blockerId)
        {
            string sql = "SELECT * FROM Blocks WHERE BlockerId = :BlockerId ORDER BY CreatedAt DESC";
            return await _connection.QueryAsync<Block>(
                sql,
                new { BlockerId = blockerId },
                _transaction
            );
        }

        // 新規ブロックを登録し、生成された BlockId を返す
        public async Task<int> CreateBlockAsync(Block block)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            string sql = @"
                INSERT INTO Blocks (BlockerId, BlockedId, CreatedAt)
                VALUES (:BlockerId, :BlockedId, :CreatedAt)
                RETURNING BlockId INTO :BlockId";

            var parameters = new DynamicParameters();
            parameters.Add("BlockerId", block.BlockerId, DbType.Int32);
            parameters.Add("BlockedId", block.BlockedId, DbType.Int32);
            parameters.Add("CreatedAt", block.CreatedAt, DbType.DateTime);
            parameters.Add("BlockId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await _connection.ExecuteAsync(sql, parameters, _transaction);
            return parameters.Get<int>("BlockId");
        }

        // 指定のブロック関係を削除
        public async Task DeleteBlockAsync(int blockerId, int blockedId)
        {
            string sql = "DELETE FROM Blocks WHERE BlockerId = :BlockerId AND BlockedId = :BlockedId";
            await _connection.ExecuteAsync(
                sql,
                new { BlockerId = blockerId, BlockedId = blockedId },
                _transaction
            );
        }
    }
}
