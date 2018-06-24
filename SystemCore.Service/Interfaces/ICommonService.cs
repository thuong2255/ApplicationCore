using System;
using System.Collections.Generic;
using System.Text;
using SystemCore.Service.ViewModels.Common;

namespace SystemCore.Service.Interfaces
{
    public interface ICommonService
    {
        FooterViewModel GetFooter();

        List<SlideViewModel> GetSildes(string groupAlias);

        SystemConfigViewModel GetSystemConfig(string code);
    }
}
