// ============================================================================
// Copyright (c) 2026 Daniel Conde Linares
// Licensed under the MIT License. See LICENSE file in the project root.
// ============================================================================

using System.Threading;
using Cysharp.Threading.Tasks;

namespace Aim4code.NanoServiceFlow.UI
{
    public interface IUITransition
    {
        UniTask PlayShowAsync(CancellationToken ct = default);
        UniTask PlayHideAsync(CancellationToken ct = default);
    }
}