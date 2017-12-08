using Threesixty.Common.Contracts.Dto.Stroller;

namespace Threesixty.Dal.Bll.Retrievers
{
    interface IStrollerRetriever
    {
        string GetFile(StrollerFileInfo strollerFileInfo);
    }
}
