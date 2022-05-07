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
  using System.Linq;
  using Anotar.Serilog;
  using Services;
  using Services.IO.HotKeys;
  using Services.IO.Keyboard;
  using Services.Sentry;
  using Services.UI.Configuration;

  // ReSharper disable once UnusedMember.Global
  // ReSharper disable once ClassNeverInstantiated.Global
  public class RegexPlugin : SentrySMAPluginBase<RegexPlugin>
  {
    #region Constructors

    public RegexPlugin() : base("https://a63c3dad9552434598dae869d2026696@sentry.io/1362046") { }

    #endregion




    #region Properties & Fields - Public

    public RegexCfg Config { get; set; }

    #endregion




    #region Properties Impl - Public

    /// <inheritdoc />
    public override string Name => "Regex";

    public override bool HasSettings => true;

    #endregion




    #region Methods Impl

    /// <inheritdoc />
    protected override void OnPluginInitialized()
    {
      LoadConfigOrDefault();

      base.OnPluginInitialized();
    }

    /// <inheritdoc />
    protected override void OnSMStarted(bool wasSMAlreadyStarted)
    {
      SynchronizeHotkeys();

      base.OnSMStarted(wasSMAlreadyStarted);
    }

    /// <inheritdoc />
    public override void ShowSettings()
    {
      ConfigurationWindow.ShowAndActivate(null, HotKeyManager.Instance);
    }

    #endregion




    #region Methods

    private void SynchronizeHotkeys()
    {
      foreach (var regexEntry in Config.Entries)
      {
        if (Svc.HotKeyManager.HotKeys.Any(hk => hk.Id == regexEntry.Name))
          continue;

        Svc.HotKeyManager.RegisterGlobal(
          regexEntry.Name,
          regexEntry.Name,
          HotKeyScopes.SMBrowser,
          null,
          () => ExecuteRegex(regexEntry));
      }
    }

    [LogToErrorOnException]
    private void ExecuteRegex(RegexEntry re)
    {
      var ctrlHtml = Svc.SM.UI.ElementWdw.ControlGroup.GetFirstHtmlControl();

      if (ctrlHtml == null)
        return;

      var html = ctrlHtml.Text;

      ctrlHtml.Text = re.Regex.Replace(html, re.Replacement);
    }

    private void LoadConfigOrDefault()
    {
      Config = Svc.Configuration.Load<RegexCfg>();

      if (Config == null)
      {
        Config = RegexCfg.Default;
        Svc.Configuration.Save(Config);
      }
    }

    #endregion
  }
}
