﻿using ACMESharp.POSH.Util;
using System;
using System.Collections.Generic;

namespace ACMESharp.POSH.Vault
{
    public class IdentifierInfo : IIdentifiable
    {
        public Guid Id
        { get; set; }

        public string Alias
        { get; set; }

        public string Label
        { get; set; }

        public string Memo
        { get; set; }

        public Guid RegistrationRef
        { get; set; }

        public string Dns
        { get; set; }

        public AuthorizationState Authorization
        { get; set; }

        public AuthorizationState AuthorizationUpdate
        { get; set; }

        public Dictionary<string, AuthorizeChallenge> Challenges
        { get; set; }

        public Dictionary<string, DateTime?> ChallengeCompleted
        { get; set; }
    }
}
