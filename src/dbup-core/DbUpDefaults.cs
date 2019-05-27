using System.Text;

namespace DbUp
{
    public static class DbUpDefaults
    {
        /// <summary>
        /// The default encoding with which files are read and written.
        /// </summary>
        public static Encoding DefaultEncoding = Encoding.UTF8;

        /// <summary>
        /// The default order group everything is run in. It is set to 100 to allow for scripts to run before the "standard" deployment as well as handle scripts to run after the deployment
        /// </summary>
        public static int DefaultRunGroupOrder = 100;
    }
}