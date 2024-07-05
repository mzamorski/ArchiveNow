using ArchiveNow.Actions.Core.Result;

namespace ArchiveNow.Actions.Core
{
    public class TestAction : AfterFinishedActionBase
    {
        public override string Description => "Test";
        
        public override IAfterFinishedActionResult Execute(IAfterFinishedActionContext context)
        {
           

            


            return new AfterFinishedActionResult(true, context.InputPath);
        }
    }
}