﻿using System;
using System.IO;
using Casbin.AspNetCore.Abstractions;
using Microsoft.Extensions.Options;
using NetCasbin;
using NetCasbin.Model;

namespace Casbin.AspNetCore.Authorization
{
    public class DefaultCasbinModelProvider : ICasbinModelProvider
    {
        private readonly string _fallbackModelPath = "model.conf";
        private readonly IOptions<CasbinAuthorizationOptions> _options;
        private Model? _model;

        public DefaultCasbinModelProvider(IOptions<CasbinAuthorizationOptions> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public virtual Model? GetModel()
        {
            if (_model is not null)
            {
                return _model;
            }

            string? modelPath = _options.Value.DefaultModelPath;

            if (string.IsNullOrWhiteSpace(modelPath))
            {
                if (_options.Value.DefaultEnforcerFactory is not null)
                {
                    return null;
                }
                modelPath = _fallbackModelPath;
            }

            if (!File.Exists(modelPath))
            {
                throw new FileNotFoundException("Can not find the model file path.", modelPath);
            }

            // it will changed at next Casbin.NET version (v1.3.2 or later)
            _model ??= CoreEnforcer.NewModel(_options.Value.DefaultModelPath, null);
            return _model;
        }
    }
}
