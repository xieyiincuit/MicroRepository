﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using XieyiES.Api.Domain;
using XieyiESLibrary.Provider;

namespace XieyiES.Api.Controllers
{
    [ApiController]
    [Route("api/v1/test")]
    public class TestController : ControllerBase
    {
        private readonly IESRepository _elasticClient;

        public TestController(IESRepository elasticClient)
        {
            _elasticClient = elasticClient;
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <returns></returns>
        [HttpPost("userwallet")]
        public async Task<IActionResult> InsertAsync()
        {
            var user = new UserWallet
            {
                UserId = "A112312312311",
                UserName = $"U{DateTime.Now.Second.ToString()}",
                CreateTime = DateTime.Now,
                Money = 110m
            };
            await _elasticClient.InsertAsync(user);
            return Ok(user);
        }

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <returns></returns>
        [HttpPost("userwallets")]
        public async Task<IActionResult> InsertRangeAsync()
        {
            var users = new List<UserWallet>
            {
                new UserWallet
                {
                    UserId = "B123123123",
                    UserName = $"U{DateTime.Now.Second.ToString()}",
                    CreateTime = DateTime.Now,
                    Money = 80m
                },
                new UserWallet
                {
                    UserId = "B4564123156",
                    UserName = $"U{DateTime.Now.Second.ToString()}",
                    CreateTime = DateTime.Now,
                    Money = 90m
                }
            };
            await _elasticClient.InsertRangeAsync(users);
            return Ok(users);
        }
    }
}
