﻿using System;
using FluentValidation;

namespace Wfm.Web.MVC.Tests.Public.Validators
{
    public class TestValidator : InlineValidator<Person>
    {
        public TestValidator()
        {

        }

        public TestValidator(params Action<TestValidator>[] actions)
        {
            foreach (var action in actions)
            {
                action(this);
            }
        }
    }
}
