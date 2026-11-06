using System;
using AugatonLib.Runtime;
using Xunit;

namespace AugatonLib.Tests
{
    public class SelfCheckTests : IDisposable
    {
        private const string Owner = "tests";

        public void Dispose() => SelfCheck.UnregisterOwner(Owner);

        [Fact]
        public void APassingCheckIsReportedAsOk()
        {
            SelfCheck.Register(Owner, "tout va bien", () => null);

            Assert.Contains("[OK] tests : tout va bien", SelfCheck.Report());
            Assert.Contains("0 echec(s)", SelfCheck.Report());
        }

        [Fact]
        public void AFailingCheckGivesItsReason()
        {
            SelfCheck.Register(Owner, "socle", () => "introuvable");

            Assert.Contains("[ECHEC] tests : socle (introuvable)", SelfCheck.Report());
            Assert.Contains("1 echec(s)", SelfCheck.Report());
        }

        [Fact]
        public void AnExceptionCountsAsAFailureAndStopsNothing()
        {
            SelfCheck.Register(Owner, "casse", () => throw new InvalidOperationException("boum"));
            SelfCheck.Register(Owner, "suite", () => null);

            string report = SelfCheck.Report();

            Assert.Contains("InvalidOperationException : boum", report);
            Assert.Contains("[OK] tests : suite", report);
        }

        [Fact]
        public void FailuresListOnlyWhatFailed()
        {
            SelfCheck.Register(Owner, "bon", () => null);
            SelfCheck.Register(Owner, "mauvais", () => "introuvable");

            string failure = Assert.Single(SelfCheck.Failures());

            Assert.Equal("tests : mauvais (introuvable)", failure);
        }

        [Fact]
        public void AnUnregisteredOwnerDisappears()
        {
            SelfCheck.Register(Owner, "ephemere", () => null);
            SelfCheck.UnregisterOwner(Owner);

            Assert.DoesNotContain("ephemere", SelfCheck.Report());
        }

        [Fact]
        public void IncompleteRegistrationsAreIgnored()
        {
            SelfCheck.Register(Owner, null, () => null);
            SelfCheck.Register(Owner, "sans test", null);

            Assert.DoesNotContain("sans test", SelfCheck.Report());
        }
    }
}
