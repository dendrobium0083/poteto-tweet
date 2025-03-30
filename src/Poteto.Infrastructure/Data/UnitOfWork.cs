using System;
using System.Data;

using Poteto.Infrastructure.Configurations;

namespace Poteto.Infrastructure.Data
{
    public interface IUnitOfWork : IDisposable
    {
        IDbTransaction Transaction { get; }
        void Commit();
        void Rollback();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbConnection _connection;
        private IDbTransaction _transaction;
        private bool _disposed;

        public UnitOfWork(DbConnectionFactory connectionFactory)
        {
            if (connectionFactory == null)
                throw new ArgumentNullException(nameof(connectionFactory));

            _connection = connectionFactory.CreateConnection();
            _transaction = _connection.BeginTransaction();
        }

        /// <summary>
        /// 現在のトランザクションを取得します。
        /// </summary>
        public IDbTransaction Transaction => _transaction;

        /// <summary>
        /// 現在のトランザクションをコミットし、新たなトランザクションを開始します。
        /// </summary>
        public void Commit()
        {
            try
            {
                if (_transaction == null) throw new InvalidOperationException("Transaction is null");
                _transaction.Commit();
                _transaction.Dispose();
                _transaction = _connection.BeginTransaction();
            }
            catch
            {
                Rollback();
                throw;
            }
        }

        /// <summary>
        /// 現在のトランザクションをロールバックし、新たなトランザクションを開始します。
        /// </summary>
        public void Rollback()
        {
            if (_transaction == null) throw new InvalidOperationException("Transaction is null");
            _transaction.Rollback();
            _transaction.Dispose();
            _transaction = _connection.BeginTransaction();
        }

        /// <summary>
        /// リソースを解放します。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _transaction?.Dispose();
                    _connection?.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
