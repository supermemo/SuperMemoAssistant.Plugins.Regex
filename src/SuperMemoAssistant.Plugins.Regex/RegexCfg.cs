#region License & Metadata

// The MIT License (MIT)
// 
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

#endregion




namespace SuperMemoAssistant.Plugins.Regex
{
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Text.RegularExpressions;
  using Forge.Forms.Annotations;
  using Newtonsoft.Json;
  using Services.UI.Configuration;
  using Sys.ComponentModel;

  [Form(Mode = DefaultFields.None)]
  [Title("PDF Settings",
         IsVisible = "{Env DialogHostContext}")]
  [DialogAction("cancel",
                "Cancel",
                IsCancel = true)]
  [DialogAction("save",
                "Save",
                IsDefault = true,
                Validates = true)]
  public class RegexCfg : CfgBase<RegexCfg>, INotifyPropertyChangedEx
  {
    #region Constants & Statics

    public static RegexCfg Default { get; } = CreateDefault();

    #endregion




    #region Properties & Fields - Public

    public List<RegexEntry> Entries { get; set; } = new();

    #endregion




    #region Properties Impl - Public

    /// <inheritdoc />
    public bool IsChanged { get; set; }

    #endregion




    #region Methods

    private static RegexCfg CreateDefault()
    {
      return new()
      {
        Entries = new List<RegexEntry>
        {
          new RegexEntry
          {
            Name = "Wikipedia (LaTeX)",
            Pattern =
              @"<SPAN class=""mwe-math-fallback-source-inline tex""[^>]*>\$([^\r\n]+)\$</SPAN>",
            Replacement = @"[$]$1[/$]"
          }
        }
      };
    }

    #endregion




    #region Events

    /// <inheritdoc />
    public event PropertyChangedEventHandler PropertyChanged;

    #endregion
  }

  public class RegexEntry
  {
    #region Properties & Fields - Non-Public

    private string _pattern;

    #endregion




    #region Properties & Fields - Public

    public string Name { get; set; }
    public string Pattern
    {
      get => _pattern;
      set
      {
        _pattern = value;
        Regex    = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.Multiline);
      }
    }
    public string Replacement { get; set; }

    [JsonIgnore]
    public Regex Regex { get; private set; }

    #endregion
  }
}
