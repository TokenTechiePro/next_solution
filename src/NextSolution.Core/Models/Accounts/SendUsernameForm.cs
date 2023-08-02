﻿using NextSolution.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NextSolution.Core.Models.Accounts
{
    public class SendUsernameForm
    {
        public string Username { get; set; } = default!;

        [JsonIgnore]
        public ContactType UsernameType => TextHelper.GetContactType(Username);

        public class Validator : AbstractValidator<SendUsernameForm>
        {
            public Validator()
            {
                RuleFor(_ => _.Username).NotEmpty().Username();
            }
        }
    }
}
