using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Collections.Generic;
using System.Linq;
using SystemCore.Data.EF.IRepositories;
using SystemCore.Data.Entities;
using SystemCore.Service.Interfaces;
using SystemCore.Service.ViewModels.Common;
using SystemCore.Utilities.Constants;

namespace SystemCore.Service.Implementations
{
    public class CommonService : ICommonService
    {
        private readonly IFooterRepository _footerRepository;
        private readonly ISlideRepository _slideRepository;
        private readonly ISystemConfigRepository _systemConfigRepository;

        public CommonService(
            IFooterRepository footerRepository,
            ISlideRepository slideRepository,
            ISystemConfigRepository systemConfigRepository)
        {
            _footerRepository = footerRepository;
            _slideRepository = slideRepository;
            _systemConfigRepository = systemConfigRepository;
        }

        public FooterViewModel GetFooter()
        {
            return Mapper.Map<Footer, FooterViewModel>(_footerRepository.FindBySingle(x => x.Id == CommonConstants.DefaultFooterId));
        }

        public List<SlideViewModel> GetSildes(string groupAlias)
        {
            return _slideRepository.FindAll(x => x.GroupAlias == groupAlias).ProjectTo<SlideViewModel>().ToList();
        }

        public SystemConfigViewModel GetSystemConfig(string code)
        {
            return Mapper.Map<SystemConfig, SystemConfigViewModel>(_systemConfigRepository.FindBySingle(x => x.Id == code));
        }
    }
}