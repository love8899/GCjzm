﻿using System;
using FluentValidation;
using FluentValidation.Attributes;
using FluentValidation.Internal;
using Wfm.Core.Infrastructure;

namespace Wfm.Web.Framework
{
    public class WfmValidatorFactory : AttributedValidatorFactory
    {
        private readonly InstanceCache _cache = new InstanceCache();
        public override IValidator GetValidator(Type type)
        {
            if (type != null)
            {
                var attribute = (ValidatorAttribute)Attribute.GetCustomAttribute(type, typeof(ValidatorAttribute));
                if ((attribute != null) && (attribute.ValidatorType != null))
                {
                    //validators can depend on some customer specific settings (such as working language)
                    //that's why it was considered we do not cache validators
                    var instance = _cache.GetOrCreateInstance(attribute.ValidatorType,
                                               x => EngineContext.Current.ContainerManager.ResolveUnregistered(x));
                    //var instance = EngineContext.Current.ContainerManager.ResolveUnregistered(attribute.ValidatorType);
                    return instance as IValidator;
                }
            }
            return null;

        }
    }
}