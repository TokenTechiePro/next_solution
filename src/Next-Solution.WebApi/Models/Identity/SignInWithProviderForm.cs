﻿using AutoMapper;
using FluentValidation;
using Next_Solution.WebApi.Data.Entities.Identity;
using Next_Solution.WebApi.Providers.ModelValidator;
using Next_Solution.WebApi.Providers.Validation;

namespace Next_Solution.WebApi.Models.Identity
{
    public class SignInWithProviderForm
    {
        public SignInWithProvider Provider { get; set; }

        public string ProviderKey { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string? LastName { get; set; }

        public string Username { get; set; } = null!;

        private ContactType? usernameType;

        public ContactType? UsernameType
        {
            get
            {
                usernameType ??= (!string.IsNullOrWhiteSpace(Username) ? ValidationHelper.DetermineContactType(Username) : null);
                return usernameType;
            }
            set => usernameType = value;
        }
    }

    public class SignInWithoutPasswordFormValidator : AbstractValidator<SignInWithProviderForm>
    {
        public SignInWithoutPasswordFormValidator()
        {
            RuleFor(_ => _.FirstName).NotEmpty().MaximumLength(256);

            RuleFor(_ => _.Username).NotEmpty().DependentRules(() =>
            {
                When(_ => _.UsernameType!.Value == ContactType.Email, () =>
                {
                    RuleFor(_ => _.Username).Email().WithName("Email");
                });

                When(_ => _.UsernameType!.Value == ContactType.PhoneNumber, () =>
                {
                    RuleFor(_ => _.Username).PhoneNumber().WithName("Phone number");
                });
            });
        }
    }

    public class SignInWithoutPasswordProfile : Profile
    {
        public SignInWithoutPasswordProfile()
        {
            CreateMap<SignInForm, User>()
                .ForMember(_ => _.UserName, _ => _.Ignore())
                .ForMember(_ => _.Email, _ => _.Ignore())
                .ForMember(_ => _.PhoneNumber, _ => _.Ignore());
        }
    }

    public enum SignInWithProvider
    {
        Google,
        Facebook
    }
}