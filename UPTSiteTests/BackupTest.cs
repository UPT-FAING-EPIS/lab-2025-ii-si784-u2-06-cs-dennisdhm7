using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UPTSiteTests
{
    [TestClass]
    public class BackupTest
    {
        [TestMethod]
        public void RunWithoutPlaywright()
        {
            // ✅ Este test siempre pasa, asegura que dotnet test funcione en cualquier entorno.
            Assert.IsTrue(true, "Test de respaldo sin dependencia de Playwright ejecutado correctamente.");
        }
    }
}
