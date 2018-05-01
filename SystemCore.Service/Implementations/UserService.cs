using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemCore.Data.Entities;
using SystemCore.Service.Interfaces;
using SystemCore.Service.ViewModels.System;
using SystemCore.Utilities.Dtos;

namespace SystemCore.Service.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;

        public UserService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> AddAsync(UserVm userVm)
        {
            var user = new AppUser
            {
                UserName = userVm.UserName,
                Avatar = userVm.Avatar,
                Email = userVm.Email,
                DateCreated = DateTime.Now,
                PhoneNumber = userVm.PhoneNumber,
                FullName = userVm.FullName,
            };

            var result = await _userManager.CreateAsync(user, userVm.Password);

            if (result.Succeeded && userVm.Roles.Count > 0)
            {
                var appUser = await _userManager.FindByNameAsync(user.UserName);

                if (appUser != null)
                    await _userManager.AddToRolesAsync(appUser, userVm.Roles);
            }
            return true;
        }

        public async Task DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            await _userManager.DeleteAsync(user);
        }

        public async Task<List<UserVm>> GetAllAsync()
        {
            return await _userManager.Users.ProjectTo<UserVm>().ToListAsync();
        }

        public PagedResult<UserVm> GetAllPagingAsync(string keyword, int page, int pageSize)
        {
            var query = _userManager.Users;

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.UserName.Contains(keyword)
                        || x.FullName.Contains(keyword)
                        || x.Email.Contains(keyword));
            }

            int totalRow = query.Count();

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            var data = query.Select(x => new UserVm
            {
                UserName = x.UserName,
                Avatar = x.Avatar,
                BirthDay = x.BirthDay.ToString(),
                Email = x.Email,
                FullName = x.FullName,
                Id = x.Id,
                PhoneNumber = x.PhoneNumber,
                Status = x.Status,
                DateCreated = x.DateCreated
            }).ToList();

            var PaginationSet = new PagedResult<UserVm>
            {
                Results = data,
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = totalRow
            };
            return PaginationSet;
        }

        public async Task<UserVm> GetById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var roles = await _userManager.GetRolesAsync(user);

            var userVm = Mapper.Map<AppUser, UserVm>(user);

            userVm.Roles = roles.ToList();

            return userVm;
        }

        public async Task UpdateAsync(UserVm userVm)
        {
            var user = await _userManager.FindByIdAsync(userVm.Id.ToString());

            var currentRoles = await _userManager.GetRolesAsync(user);

            //Add Role To User
            var result = await _userManager.AddToRolesAsync(user, userVm.Roles.Except(currentRoles).ToArray());

            if (result.Succeeded)
            {
                //Remove role don't have UserVm but have in Db
                string[] roleRemove = currentRoles.Except(userVm.Roles).ToArray();
                await _userManager.RemoveFromRolesAsync(user, roleRemove);

                //Update User Detail
                user.FullName = userVm.FullName;
                user.Status = userVm.Status;
                user.Email = userVm.Email;
                user.PhoneNumber = userVm.PhoneNumber;
                user.Avatar = userVm.Avatar;
                await _userManager.UpdateAsync(user);
            }
        }
    }
}