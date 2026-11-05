using AugatonLib.Runtime;
using Xunit;

namespace AugatonLib.Tests
{
    public class LoadMonitorTests
    {
        private static bool Report(LoadMonitor monitor, float tps) => monitor.Report(tps, 30f, 45f, 3);

        [Fact]
        public void ASingleDipIsNotAnAlert()
        {
            LoadMonitor monitor = new LoadMonitor();

            Assert.False(Report(monitor, 10f));
            Assert.False(Report(monitor, 60f));
            Assert.False(Report(monitor, 10f));
            Assert.False(monitor.IsDegraded);
        }

        [Fact]
        public void EnoughLowChecksInARowRaiseTheAlertOnce()
        {
            LoadMonitor monitor = new LoadMonitor();

            Report(monitor, 10f);
            Report(monitor, 10f);

            Assert.True(Report(monitor, 10f));
            Assert.False(Report(monitor, 10f));
            Assert.True(monitor.IsDegraded);
            Assert.Equal(1, monitor.Episodes);
        }

        [Fact]
        public void RecoveryNeedsTheHigherThreshold()
        {
            LoadMonitor monitor = new LoadMonitor();

            for (int i = 0; i < 3; i++)
                Report(monitor, 10f);

            Assert.False(Report(monitor, 40f));
            Assert.True(monitor.IsDegraded);
            Assert.True(Report(monitor, 50f));
            Assert.False(monitor.IsDegraded);
        }

        [Fact]
        public void TheLowestValueIsKeptUntilReset()
        {
            LoadMonitor monitor = new LoadMonitor();

            Report(monitor, 40f);
            Report(monitor, 20f);
            Report(monitor, 60f);

            Assert.Equal(20f, monitor.Lowest);

            monitor.Reset();

            Assert.Equal(0, monitor.Episodes);
            Assert.Equal(float.MaxValue, monitor.Lowest);
        }
    }
}
