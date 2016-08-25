using Akka.Actor;
using Akka.TestKit.NUnit3;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepthFirstSearchOfATree.Tests
{
    public class UserIdentityActor : ReceiveActor
    {

        public UserIdentityActor()
        {
            Receive<CreateUserWithValidUserInfo>(create =>
            {
                //create user here
                Sender.Tell(new OperationResult() { Successful = true });
            });
        }

        public class OperationResult
        {
            public bool Successful { get; set; }
        }

        public class CreateUserWithValidUserInfo
        {
        }
    }

    [TestFixture]
    public class GettingStartedTest : TestKit
    {
        [Test]
        public void UserIdentityActor_should_confirm_user_creation_success()
        {
            var identity = Sys.ActorOf(Props.Create(() => new UserIdentityActor()));
            identity.Tell(new UserIdentityActor.CreateUserWithValidUserInfo());
            var result = ExpectMsg<UserIdentityActor.OperationResult>().Successful;
            Assert.True(result);
        }
    }
}
