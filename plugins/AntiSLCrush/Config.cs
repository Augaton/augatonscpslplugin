// Copyright (c) 2026 ui_2506.
// Modified by Augaton.
// Licensed under the ui_2506 Non-Commercial License.
// See LICENSE.md in this folder for full license terms.

using System.ComponentModel;

namespace AntiSLCrush
{
    public sealed class Config
    {
        [Description("Active les logs de debug.")]
        public bool Debug { get; set; } = false;
    }
}
