using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbUp.Oracle.Helpers
{
    /// <summary>
    /// Temporary Oracle Database. Does not create new database instance.
    /// </summary>
    public class TemporaryOracleDatabase : IDisposable
    {
        // Track whether Dispose has been called. 
        private bool disposed = false;
        private readonly string connectionString;

        public TemporaryOracleDatabase(string connectionString)
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString
        {
            get { return connectionString; }
        }

        // Implement IDisposable. 
        // Do not make this method virtual. 
        // A derived class should not be able to override this method. 
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method. 
            // Therefore, you should call GC.SupressFinalize to 
            // take this object off the finalization queue 
            // and prevent finalization code for this object 
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios. 
        // If disposing equals true, the method has been called directly 
        // or indirectly by a user's code. Managed and unmanaged resources 
        // can be disposed. 
        // If disposing equals false, the method has been called by the 
        // runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed. 
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources. 
                if (disposing)
                {
                }

                // Note disposing has been done.
                disposed = true;

            }
        }

    }
}
