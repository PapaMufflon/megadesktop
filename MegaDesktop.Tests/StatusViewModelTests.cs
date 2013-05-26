using MegaDesktop.Services;
using MegaDesktop.ViewModels;
using NUnit.Framework;

namespace MegaDesktop.Tests
{
    [TestFixture]
    public class StatusViewModelTests
    {
        [Test]
        public void Setting_a_status_makes_that_the_current_status()
        {
            var target = new StatusViewModel();

            target.SetStatus(Status.Loaded);

            Assert.That(target.CurrentStatus, Is.EqualTo(Status.Loaded));
        }

        [Test]
        public void Changing_a_status_changes_also_the_message()
        {
            var target = new StatusViewModel();

            target.SetStatus(Status.Loaded);

            var message = target.Message;

            target.SetStatus(Status.Communicating);

            Assert.That(message, Is.Not.EqualTo(target.Message));
        }

        [Test]
        public void Changing_the_status_raises_the_CurrentStatusChanged_event()
        {
            var raised = false;
            var target = new StatusViewModel();

            target.CurrentStatusChanged += (s, e) => raised = true;

            target.SetStatus(Status.Loaded);

            Assert.That(raised, Is.True);
        }
    }
}