using UnityEngine;
using System.Collections;

/// <summary>
/// uWebKit Project Configuration
/// </summary>
public static class UWKConfig
{

    /// <summary>
    /// Specify absolute path to the root cache folder, if not specified a default folder will be used
    /// </summary>
    public static string RootCacheFolder = "";

    /// <summary>
    /// Specify the CacheName for the application, will be created under RootCacheFolder
    /// </summary>
    public static string CacheName = "uWebKit";

    /// <summary>
    /// Set the User Agent to the provided string
    /// </summary>
    public static string UserAgent = "";

    /// <summary>
    /// Set the product part of the User Agent, except replace Chromium product info with provided string
	/// For example "MyBrowser/3.0"
    /// </summary>
	public static string UserAgentProductVersion = "uWebKit/3.0";

    /// <summary>
    /// Set the debugging port which can be used to debug uWebKit WebViews from Chrome by visiting "localhost:3335"
    /// </summary>
    public static int ChromiumDebugPort = 3335;

    /// <summary>
    /// Controls whether web security restrictions (same-origin policy) will be
    /// enforced. Disabling this setting is not recommend as it will allow risky
    /// security behavior such as cross-site scripting (XSS).
    /// </summary>
    public static bool WebSecurity = true;


}
