﻿using System;
using System.Threading.Tasks;
using APICore.Common.DTO.Request;
using APICore.Data.Entities;
using APICore.Data.UoW;
using APICore.Services.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace APICore.Services.Impls
{
    public class SettingService : ISettingService
    {
        private readonly IUnitOfWork _uow;

        private readonly IStringLocalizer<IAccountService> _localizer;

        public SettingService(IUnitOfWork uow, IStringLocalizer<IAccountService> localizer)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public async Task<string> GetSettingAsync(string settingKey)
        {
            var setting = await _uow.SettingRepository.FindBy(s => s.Key == settingKey)
                                    .FirstOrDefaultAsync();
            if (setting == null)
            {
                throw new SettingNotFoundException(_localizer);
            }

            return setting.Value;
        }

        public async Task<Setting> SetSettingAsync(SettingRequest settingRequest)
        {
            var result = await _uow.SettingRepository.FindBy(s => s.Key == settingRequest.Key)
                                   .FirstOrDefaultAsync();

            if (result != null)
            {
                result.Value = settingRequest.Value;
                _uow.SettingRepository.Update(result);
            }
            else
            {
                result = new Setting {Key = settingRequest.Key, Value = settingRequest.Value};
                await _uow.SettingRepository.AddAsync(result);
            }

            await _uow.CommitAsync();

            return result;
        }
    }
}
