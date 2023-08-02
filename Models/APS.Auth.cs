/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Developer Advocacy and Support
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

using System;
using System.Threading.Tasks;
using Autodesk.Forge;
using Autodesk.Forge.Api;

namespace Autodesk.Das.Models
{
    public record Token(string AccessToken, DateTime ExpiresAt);

    public partial class APS
    {
        private Token _internalTokenCache;
        private Token _publicTokenCache;

        private async Task<Token> GetToken(Scope[] scopes)
        {
            dynamic auth = await new TwoLeggedApiV2().AuthenticateAsync(_clientId, _clientSecret, "client_credentials", scopes);
            return new Token(auth.access_token, DateTime.UtcNow.AddSeconds(auth.expires_in));
        }

        public async Task<Token> GetPublicToken()
        {
            if (_publicTokenCache == null || _publicTokenCache.ExpiresAt < DateTime.UtcNow)
                _publicTokenCache = await GetToken(new Scope[] { Scope.ViewablesRead });
            return _publicTokenCache;
        }

        private async Task<Token> GetInternalToken()
        {
            if (_internalTokenCache == null || _internalTokenCache.ExpiresAt < DateTime.UtcNow)
                _internalTokenCache = await GetToken(new Scope[] { Scope.BucketCreate, Scope.BucketRead, Scope.DataRead, Scope.DataWrite, Scope.DataCreate });
            return _internalTokenCache;
        }
    }
}