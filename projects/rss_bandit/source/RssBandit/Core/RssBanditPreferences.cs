﻿#region Version Info Header
/*
 * $Id$
 * $HeadURL$
 * Last modified by $Author$
 * Last modified at $Date$
 * $Revision$
 */
#endregion

#region usings
using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Collections.Generic;
using NewsComponents;
using NewsComponents.Utils;
using RssBandit.ViewModel;
using Logger = RssBandit.Common.Logging;
using RssBandit.WinGui.Utility;
using RssBandit.AppServices;
#endregion

namespace RssBandit 
{
	/// <summary>
	/// RssBanditPreferences manages 
	/// all the Bandit specific user preferences.
	/// </summary>
	[Serializable]
	public class RssBanditPreferences : BindableBase, ISerializable, IUserPreferences
	{
		#region bool instance variables
		/// <summary>
		/// To get rid of all the bool variables,
		/// we now use one store to track the bool states.
		/// </summary>
		[Flags,Serializable]
		private enum OptionalFlags:long
		{
			AllOff = 0,
			CustomProxy = 0x1,
			TakeIEProxySettings = 0x2,
			ByPassProxyOnLocal = 0x4,
			ProxyCustomCredentials = 0x8,
			UseRemoteStorage = 0x10,
			ReUseFirstBrowserTab = 0x20,
			NewsItemOpenLinkInDetailWindow = 0x40,
			MarkFeedItemsReadOnExit = 0x80,
			RefreshFeedsOnStartup = 0x100,
			AllowJavascriptInBrowser = 0x200,
			AllowJavaInBrowser = 0x400, 
			AllowActiveXInBrowser = 0x800,
			AllowBGSoundInBrowser = 0x1000, 
			AllowVideoInBrowser = 0x2000, 
			AllowImagesInBrowser = 0x4000,
			ShowNewItemsReceivedBalloon = 0x8000,
			BuildRelationCosmos = 0x10000,
			OpenNewTabsInBackground = 0x20000,
			DisableFavicons = 0x40000,
			AddPodcasts2ITunes = 0x80000,
			AddPodcasts2WMP    = 0x100000,
			AddPodcasts2Folder = 0x200000,
			SinglePodcastPlaylist = 0x400000,
			AllowAppEventSounds = 0x800000,
			ShowAllNewsItemsPerPage = 0x1000000,
			DisableAutoMarkItemsRead = 0x2000000,
			DownloadEnclosures = 0x4000000,
			EnclosureAlert = 0x8000000,
			CreateSubfoldersForEnclosures = 0x100000000,
		}

		private OptionalFlags allOptionalFlags;
		#endregion

		#region other instance variables

		private static readonly log4net.ILog _log = Logger.Log.GetLogger(typeof(RssBanditPreferences));
		
        //new 2.0.x
		private int refreshRate = FeedSource.DefaultRefreshRate;	
        private TextSize readingPaneTextSize = TextSize.Medium;

		//new 1.5.x
		private int numNewsItemsPerPage = 10;

		// new: 1.3.x
		private string userIdentityForComments = String.Empty;
		private string ngosSyncToken = String.Empty;

		// old: 1.2.x; see RssBanditApplication.CheckAndMigrateSettingsAndPreferences() 
		private string referer = String.Empty;
		private string userName = String.Empty;
		private string userMailAddress = String.Empty;

		private string[] proxyBypassList = new string[]{};
		private string proxyAddress = String.Empty;
		private int proxyPort = 0;
		private string proxyUser = String.Empty;
		private string proxyPassword = String.Empty;

		private string remoteStorageUserName = String.Empty;
		private string remoteStoragePassword = String.Empty;
		private RemoteStorageProtocolType remoteStorageProtocol = RemoteStorageProtocolType.UNC;
		private string remoteStorageLocation = String.Empty;
		private string enclosureFolder = String.Empty;
		private int numEnclosuresToDownloadOnNewFeed;
		private int enclosureCacheSize;
		private string podcastFolder = String.Empty;
		private string podcastFileExtensions = String.Empty;

		private string singlePlaylistName    = String.Empty;

		private string newsItemStylesheetFile = String.Empty;
		private HideToTray hideToTrayAction = HideToTray.OnMinimize;

		private Font normalFont;
		private Font unreadFont;
		private Font flagFont;
		private Font errorFont;
		private Font referrerFont;
		private Font newCommentsFont;
		private Color normalFontColor = FontColorHelper.DefaultNormalColor;
		private Color unreadFontColor = FontColorHelper.DefaultUnreadColor;
		private Color flagFontColor = FontColorHelper.DefaultHighlightColor;
		private Color errorFontColor = FontColorHelper.DefaultFailureColor;
		private Color referrerFontColor = FontColorHelper.DefaultReferenceColor;
		private Color newCommentsColor = FontColorHelper.DefaultNewCommentsColor;

		// general max item age: 90 days:
		private TimeSpan maxItemAge = TimeSpan.FromDays(90);	

		private BrowserBehaviorOnNewWindow browserBehaviorOnNewWindow = BrowserBehaviorOnNewWindow.OpenDefaultBrowser;
		private string browserCustomExecOnNewWindow = String.Empty;

		private DisplayFeedAlertWindow feedAlertWindow = DisplayFeedAlertWindow.AsConfiguredPerFeed;
		#endregion

		#region public properties

		/// <summary>
		/// Gets or sets the refresh rate in millisecs.
		/// </summary>
		/// <value>The refresh rate.</value>
		internal int RefreshRate
		{
			[DebuggerStepThrough]
			get { return refreshRate; }
			set
			{
				SetProperty(ref refreshRate, value);
			}
		}

        /// <summary>
        /// Gets/Sets the size of the text in the reading pane
        /// </summary>
        public TextSize ReadingPaneTextSize
        {
            [DebuggerStepThrough]
            get { return readingPaneTextSize; }
            set
            {
	            SetProperty(ref readingPaneTextSize, value);
            }	
        }

		/// <summary>
		/// Gets/Sets the number of news items to display per page in the newspaper view
		/// </summary>
		public int NumNewsItemsPerPage{
			[DebuggerStepThrough]
			get { return numNewsItemsPerPage; }
			set 
			{
				SetProperty(ref numNewsItemsPerPage, value);
			}		
		}


		/// <summary>
		/// Gets/Sets the Newsgator Online sync token.
		/// </summary>
		public string NgosSyncToken {
			[DebuggerStepThrough]
			get { return ngosSyncToken; }
			set 
			{ 
				SetProperty(ref ngosSyncToken, value);
			}
		}

		/// <summary>
		/// Gets/Sets the user identity used to post feed comments.
		/// </summary>
		public string UserIdentityForComments {
			[DebuggerStepThrough]
			get { return userIdentityForComments; }
			set 
			{
				SetProperty(ref userIdentityForComments, value);
			}
		}

		#region kept for migration reasons only
		/// <summary>
		/// Obsolete. Do not use it anymore!
		/// Used only to migrate old values to the new structure UserIdentity.
		/// </summary>
		public string Referer 
		{
			[DebuggerStepThrough]
			get {	return referer;		}
			set {	referer = value;	}
		}

		/// <summary>
		/// Obsolete. Do not use it anymore!
		/// Used only to migrate old values to the new structure UserIdentity.
		/// </summary>
		public string UserName {
			[DebuggerStepThrough]
			get {	return userName;	}
			set {	userName = value;	}
		}

		/// <summary>
		/// Obsolete. Do not use it anymore!
		/// Used only to migrate old values to the new structure UserIdentity.
		/// </summary>
		public string UserMailAddress {
			[DebuggerStepThrough]
			get {	return userMailAddress;		}
			set {	userMailAddress = value;	}
		}
		#endregion

		/// <summary>
		/// Sets/Get a value that control if feeds should be refreshed from the original
		/// source on startup of the application.
		/// </summary>
		public bool FeedRefreshOnStartup 
		{			
			[DebuggerStepThrough]
			get {	return GetOption(OptionalFlags.RefreshFeedsOnStartup); }
			set {	
				SetOption(OptionalFlags.RefreshFeedsOnStartup, value);		
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Gets/Set a value to control if the application have to use a proxy to
		/// request feeds.
		/// </summary>
		public bool UseProxy {
			[DebuggerStepThrough]
			get {	return GetOption(OptionalFlags.CustomProxy);		}
			set {	
				SetOption(OptionalFlags.CustomProxy, value);
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// If <see cref="UseProxy">UseProxy</see> is set to true, this option is used
		/// to force a take over the proxy settings from and installed Internet Explorer.
		/// (Including automatic proxy configuration).
		/// </summary>
		public bool UseIEProxySettings {
			[DebuggerStepThrough]
			get { return GetOption(OptionalFlags.TakeIEProxySettings);	}
			set { 
				SetOption(OptionalFlags.TakeIEProxySettings, value);
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Gets/Set the value if the used proxy should bypass requests
		/// for local (intranet) servers.
		/// </summary>
		public bool BypassProxyOnLocal {
			[DebuggerStepThrough]
			get {	return GetOption(OptionalFlags.ByPassProxyOnLocal);		}
			set {	
				SetOption(OptionalFlags.ByPassProxyOnLocal, value);
				OnPropertyChanged();
			}
		}


		/// <summary>
		/// Gets/Sets the value that indicates whether a news item should be automatically 
		/// marked as read when viewed in the newspaper view
		/// </summary>
		public bool MarkItemsAsReadWhenViewed { 
			[DebuggerStepThrough]
			get {	return !GetOption(OptionalFlags.DisableAutoMarkItemsRead);		}
			set {	
				SetOption(OptionalFlags.DisableAutoMarkItemsRead, !value);
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Gets/Set the value that indicates whether a limited number of news items
		/// should be displayed per page in the newspaper view. 
		/// </summary>
		public bool LimitNewsItemsPerPage {
			[DebuggerStepThrough]
			get {	return !GetOption(OptionalFlags.ShowAllNewsItemsPerPage);		}
			set {	
				SetOption(OptionalFlags.ShowAllNewsItemsPerPage, !value);
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Sets/Get a list of servers/web addresses to bypass by the used proxy.
		/// </summary>
		public string[] ProxyBypassList 
		{
			[DebuggerStepThrough]
			get {	return proxyBypassList;			}
			set 
			{	
				SetProperty(ref proxyBypassList, value);
			}
		}

		/// <summary>
		/// Sets/Get the proxy address.
		/// </summary>
		public string ProxyAddress {
			[DebuggerStepThrough]
			get {	return proxyAddress;	}
			set 
			{
				SetProperty(ref proxyAddress, value);
			}
		}

		/// <summary>
		/// Sets/Get the proxy port number.
		/// </summary>
		public int ProxyPort {
			[DebuggerStepThrough]
			get {	return proxyPort;		}
			set 
			{
				SetProperty(ref proxyPort, value);
			}
		}

		/// <summary>
		/// Sets/Get a value indicating if the proxy have to use 
		/// custom credentials (proxy needs authentication).
		/// </summary>
		public bool ProxyCustomCredentials {
			[DebuggerStepThrough]
			get {	return GetOption(OptionalFlags.ProxyCustomCredentials);		}
			set {	
				SetOption(OptionalFlags.ProxyCustomCredentials, value);		
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Sets/Get the proxy custom credential user name.
		/// </summary>
		public string ProxyUser {
			[DebuggerStepThrough]
			get {	return proxyUser;		}
			set 
			{
				SetProperty(ref proxyUser, value);
			}
		}

		/// <summary>
		/// Sets/Get the proxy custom credential user password.
		/// </summary>
		public string ProxyPassword {
			[DebuggerStepThrough]
			get {	return proxyPassword;		}
			set 
			{
				SetProperty(ref proxyPassword, value);
			}
		}
		
		/// <summary>
		/// Sets/Get the global news item formatter stylesheet 
		/// (filename excluding path name)
		/// </summary>
		public string NewsItemStylesheetFile {
			[DebuggerStepThrough]
			get {	return newsItemStylesheetFile;		}
			set 
			{
				SetProperty(ref newsItemStylesheetFile, value);
			}
		}


		/// <summary>
		/// Sets/Get the user-specified name for the WMP or iTunes playlist that will 
		/// contain all podcasts from RSS Bandit. 
		/// </summary>
		public string SinglePlaylistName {
			[DebuggerStepThrough]
			get {	return singlePlaylistName;		}
			set 
			{
				SetProperty(ref singlePlaylistName, value);
			}
		}
		

		/// <summary>
		/// Sets/Get a value to control if the first opened web browser Tab should
		/// be reused or not.
		/// </summary>
		public bool ReuseFirstBrowserTab {
			[DebuggerStepThrough]
			get {	return GetOption(OptionalFlags.ReUseFirstBrowserTab);		}
			set {	
				SetOption(OptionalFlags.ReUseFirstBrowserTab, value);	
				OnPropertyChanged();
			}
		}	

		/// <summary>
		/// Sets/Get a value to control if the new browser tabs should be opened 
		/// in the background.
		/// </summary>
		public bool OpenNewTabsInBackground {
			[DebuggerStepThrough]
			get {	return GetOption(OptionalFlags.OpenNewTabsInBackground);		}
			set {	
				SetOption(OptionalFlags.OpenNewTabsInBackground, value);	
				OnPropertyChanged();
			}
		}	

		/// <summary>
		/// Gets or sets a value indicating whether to allow application
		/// event sounds.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [allow app event sounds]; otherwise, <c>false</c>.
		/// </value>
		public bool AllowAppEventSounds {
			[DebuggerStepThrough]
			get {	return GetOption(OptionalFlags.AllowAppEventSounds);		}
			set {	
				SetOption(OptionalFlags.AllowAppEventSounds, value);	
				OnPropertyChanged();
			}
		}	

		///// <summary>
		///// Gets or sets a value indicating whether to run bandit as windows user logon.
		///// It directly modifies the registry value within the "Run" section and
		///// don't get persisted into preferences file.
		///// </summary>
		///// <value>
		///// 	<c>true</c> if [run bandit as windows user logon]; otherwise, <c>false</c>.
		///// </value>
		//public bool RunBanditAsWindowsUserLogon {
		//	get { return Win32.Registry.RunAtStartup; }
		//	set {
		//		if (Win32.Registry.RunAtStartup != value)
		//		{
		//			Win32.Registry.RunAtStartup = value;
		//			OnPropertyChanged();
		//		}
		//	}
		//}
		
		/// <summary>
		/// Sets/Get a value to control whether there should be a single playlist 
		/// for podcasts files. If this value is false, then podcasts are added to 
		/// a playlist with the same name as the feed. 
		/// </summary>
		public bool SinglePodcastPlaylist {
			[DebuggerStepThrough]
			get {	return GetOption(OptionalFlags.SinglePodcastPlaylist);		}
			set {	
				SetOption(OptionalFlags.SinglePodcastPlaylist, value);	
				OnPropertyChanged();
			}
		}	

		/// <summary>
		/// Sets/Get a value to control if podcasts should be moved to a specified 
		/// podcasts folder. 
		/// </summary>
		public bool AddPodcasts2Folder {
			[DebuggerStepThrough]
			get {	return GetOption(OptionalFlags.AddPodcasts2Folder);		}
			set {	
				SetOption(OptionalFlags.AddPodcasts2Folder, value);	
				OnPropertyChanged();
			}
		}	

		/// <summary>
		/// Sets/Get a value to control if a playlist in Windows Media Player should be 
		/// created when an WMP-compatible podcast is successfully downloaded
		/// </summary>
		public bool AddPodcasts2WMP {
			[DebuggerStepThrough]
			get {	return GetOption(OptionalFlags.AddPodcasts2WMP);		}
			set {	
				SetOption(OptionalFlags.AddPodcasts2WMP, value);	
				OnPropertyChanged();
			}
		}	

		/// <summary>
		/// Sets/Get a value to control if a playlist in iTunes should be 
		/// created when an iTunes-compatible podcast is successfully downloaded
		/// </summary>
		public bool AddPodcasts2ITunes {
			[DebuggerStepThrough]
			get {	return GetOption(OptionalFlags.AddPodcasts2ITunes);		}
			set {	
				SetOption(OptionalFlags.AddPodcasts2ITunes, value);	
				OnPropertyChanged();
			}
		}	

		/// <summary>
		/// Sets/Get a value to control if the favicons are used as feed icons 
		/// in the tree view.
		/// </summary>
		public bool UseFavicons {
			[DebuggerStepThrough]
			get {	return !GetOption(OptionalFlags.DisableFavicons);		}
			set {	
				SetOption(OptionalFlags.DisableFavicons, !value);	
				OnPropertyChanged();
			}
		}	
		/// <summary>
		/// Sets/Get a value to control if unread feed items should be marked as read
		/// while leaving the feed through UI navigation (to another feed/category)
		/// </summary>
		public bool MarkItemsReadOnExit {
			[DebuggerStepThrough]
			get { return GetOption(OptionalFlags.MarkFeedItemsReadOnExit);	}
			set { 
				SetOption(OptionalFlags.MarkFeedItemsReadOnExit, value);	
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Sets/Get a value to control if an news item without a description
		/// should display the (web page) content of the link target instead (if true).
		/// </summary>
		public bool NewsItemOpenLinkInDetailWindow {
			[DebuggerStepThrough]
			get { return GetOption(OptionalFlags.NewsItemOpenLinkInDetailWindow);	}
			set { 
				SetOption(OptionalFlags.NewsItemOpenLinkInDetailWindow, value);	
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Sets/Get the user action <see cref="HideToTray">HideToTray</see> 
		/// when the application should minimize to the
		/// system tray area.
		/// </summary>
		public HideToTray HideToTrayAction {
			[DebuggerStepThrough]
			get {	return hideToTrayAction;		}
			set 
			{	
				SetProperty(ref hideToTrayAction, value);
			}
		}

		/// <summary>
		/// Normal font used to render items (listview) 
		/// and feeds (tree view)
		/// </summary>
		public Font NormalFont {
			[DebuggerStepThrough]
			get {	return normalFont;		}
			set 
			{
				SetProperty(ref normalFont, value);
			}
		}

		/// <summary>
		/// Normal font color used to render items (listview) 
		/// and feeds (tree view)
		/// </summary>
		public Color NormalFontColor {
			[DebuggerStepThrough]
			get {	return normalFontColor;		}
			set 
			{
				SetProperty(ref normalFontColor, value);
			}
		}

		/// <summary>
		/// Font used to highlight unread items (listview) 
		/// and feeds (tree view)
		/// </summary>
		public Font UnreadFont {
			[DebuggerStepThrough]
			get {	return unreadFont;		}
			set 
			{
				SetProperty(ref unreadFont, value);
			}
		}

		/// <summary>
		/// Color used to highlight unread items (listview) 
		/// and feeds (tree view)
		/// </summary>
		public Color UnreadFontColor {
			[DebuggerStepThrough]
			get {	return unreadFontColor;		}
			set 
			{
				SetProperty(ref unreadFontColor, value);
			}
		}

		/// <summary>
		/// Font used to render flagged items (listview) 
		/// </summary>
		public Font FlagFont {
			[DebuggerStepThrough]
			get {	return flagFont;		}
			set 
			{
				SetProperty(ref flagFont, value);
			}
		}
		
		/// <summary>
		/// Color used to render flagged items (listview) 
		/// </summary>
		public Color FlagFontColor {
			[DebuggerStepThrough]
			get {	return flagFontColor;		}
			set 
			{
				SetProperty(ref flagFontColor, value);
			}
		}

		/// <summary>
		/// Font used to render items that refer back to the users 
		/// default identity (listview) 
		/// </summary>
		public Font ReferrerFont {
			[DebuggerStepThrough]
			get {	return referrerFont;		}
			set 
			{
				SetProperty(ref referrerFont, value);
			}
		}

		/// <summary>
		/// Color used to render items that refer back to the users 
		/// default identity (listview) 
		/// </summary>
		public Color ReferrerFontColor {
			[DebuggerStepThrough]
			get {	return referrerFontColor;	}
			set 
			{
				SetProperty(ref referrerFontColor, value);
			}
		}

		/// <summary>
		/// Font used to render items that display an error message (listview) 
		/// </summary>
		public Font ErrorFont {
			[DebuggerStepThrough]
			get {	return errorFont;		}
			set 
			{
				SetProperty(ref errorFont, value);
			}
		}

		/// <summary>
		/// Color used to render items that display an error message (listview) 
		/// </summary>
		public Color ErrorFontColor {
			[DebuggerStepThrough]
			get {	return errorFontColor;		}
			set 
			{
				SetProperty(ref errorFontColor, value);
			}
		}

		/// <summary>
		/// Font used to render items that received new comments (watched) 
		/// </summary>
		public Font NewCommentsFont {
			[DebuggerStepThrough]
			get {	return newCommentsFont;		}
			set 
			{
				SetProperty(ref newCommentsFont, value);
			}
		}

		/// <summary>
		/// Color used to render items that received new comments (watched) 
		/// </summary>
		public Color NewCommentsFontColor {
			[DebuggerStepThrough]
			get {	return newCommentsColor;		}
			set 
			{
				SetProperty(ref newCommentsColor, value);
			}
		}
		
		/// <summary>
		/// Sets/Get the TimeSpan for the global maximum news item age.
		/// You have to use TimeSpan.MinValue for the unlimited item age.
		/// </summary>
		public TimeSpan MaxItemAge {
			[DebuggerStepThrough]
			get {	return maxItemAge;	}
			set 
			{
				SetProperty(ref maxItemAge, value);
			}
		}

		/// <summary>
		/// Sets/Get the value indicating if we have to use a remote storage
		/// for sync. states.
		/// </summary>
		public bool UseRemoteStorage {
			[DebuggerStepThrough]
			get { return GetOption(OptionalFlags.UseRemoteStorage); }
			set 
			{
				SetOption(OptionalFlags.UseRemoteStorage, value);
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Sets/Get the username that may be required to access
		/// the remote storage location.
		/// </summary>
		public string RemoteStorageUserName {
			[DebuggerStepThrough]
			get { return remoteStorageUserName; }
			set
			{
				SetProperty(ref remoteStorageUserName, value);
			}
		}

		/// <summary>
		/// Sets/Get the password that may be required to access the remote
		/// storage location.
		/// </summary>
		public string RemoteStoragePassword {
			[DebuggerStepThrough]
			get { return remoteStoragePassword; }
			set
			{
				SetProperty(ref remoteStoragePassword, value);
			}
		}

		/// <summary>
		/// Sets/Get the type of remote storage to use. <see cref="RemoteStorageProtocolType"/>
		/// </summary>
		public RemoteStorageProtocolType RemoteStorageProtocol {
			[DebuggerStepThrough]
			get { return remoteStorageProtocol; }
			set
			{
				SetProperty(ref remoteStorageProtocol, value);
			}
		}

		/// <summary>
		/// Sets/Get the remote storage location. Can vary dep. on
		/// the location type (ftp, share,...)
		/// </summary>
		public string RemoteStorageLocation {
			[DebuggerStepThrough]
			get { return remoteStorageLocation; }
			set 
			{
				SetProperty(ref remoteStorageLocation, value);
			}
		}

		/// <summary>
		/// Gets or sets the enclosure download folder.
		/// </summary>
		/// <value>The enclosure folder.</value>
		public string EnclosureFolder
		{
			[DebuggerStepThrough]
			get { return enclosureFolder; }
			set
			{
				SetProperty(ref enclosureFolder, value);
			}
		}
		/// <summary>
		/// Indicates the number of enclosures which should be 
		/// downloaded automatically from a newly subscribed feed.
		/// </summary>
		public int NumEnclosuresToDownloadOnNewFeed
		{
			[DebuggerStepThrough]
			get { return numEnclosuresToDownloadOnNewFeed; }
			set
			{
				SetProperty(ref numEnclosuresToDownloadOnNewFeed, value);
			}
		}
		/// <summary>
		/// Indicates the maximum amount of space that enclosures and 
		/// podcasts can use on disk.
		/// </summary>
		public int EnclosureCacheSize
		{
			[DebuggerStepThrough]
			get { return enclosureCacheSize; }
			set
			{
				SetProperty(ref enclosureCacheSize, value);
			}
		}


		/// <summary>
		/// Sets/Get a value that control if enclosures should be downloaded
		/// </summary>
		public bool DownloadEnclosures
		{
			[DebuggerStepThrough]
			get { return GetOption(OptionalFlags.DownloadEnclosures); }
			set
			{
				SetOption(OptionalFlags.DownloadEnclosures, value);
				OnPropertyChanged();
			}
		}
		/// <summary>
		/// Sets/Get a value that control if an alert should be displayed if enclosures are downloaded
		/// </summary>
		public bool EnclosureAlert
		{
			[DebuggerStepThrough]
			get { return GetOption(OptionalFlags.EnclosureAlert); }
			set
			{
				SetOption(OptionalFlags.EnclosureAlert, value);
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Gets or sets whether  podcasts and enclosures should be downloaded to a folder 
		/// named after the feed
		/// </summary>
		public bool CreateSubfoldersForEnclosures
		{
			get
			{
				return GetOption(OptionalFlags.CreateSubfoldersForEnclosures); 
			}

			set
			{
				SetOption(OptionalFlags.CreateSubfoldersForEnclosures, value);
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Gets or sets the podcast download folder.
		/// </summary>
		/// <value>The podcast folder.</value>
		public string PodcastFolder
		{
			[DebuggerStepThrough]
			get { return podcastFolder; }
			set
			{
				SetProperty(ref podcastFolder, value);
			}
		}

		/// <summary>
		/// Gets or sets the podcast file extensions.
		/// </summary>
		/// <value>The podcast file extensions.</value>
		public string PodcastFileExtensions
		{
			[DebuggerStepThrough]
			get { return podcastFileExtensions; }
			set
			{
				SetProperty(ref podcastFileExtensions, value);
			}
		}
		
		/// <summary>
		/// Sets/Get the behavior how to handle requests to open new
		/// window(s) while browsing
		/// </summary>
		public BrowserBehaviorOnNewWindow BrowserOnNewWindow {
			[DebuggerStepThrough]
			get { return browserBehaviorOnNewWindow; }
			set 
			{
				SetProperty(ref browserBehaviorOnNewWindow, value);
			}
		}

		/// <summary>
		/// Gets/Set the executable application to start if
		/// browser requires to open a new window.
		/// </summary>
		public string BrowserCustomExecOnNewWindow  {
			[DebuggerStepThrough]
			get { return browserCustomExecOnNewWindow; }
			set 
			{
				SetProperty(ref browserCustomExecOnNewWindow, value);
			}
		}

		/// <summary>
		/// Sets/Get if Javascript should be allowed to execute
		/// </summary>
		public bool BrowserJavascriptAllowed { 
			[DebuggerStepThrough]
			get { return GetOption(OptionalFlags.AllowJavascriptInBrowser); }
			set 
			{
				SetOption(OptionalFlags.AllowJavascriptInBrowser, value);
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Sets/Get if Java should be allowed to execute
		/// </summary>
		public bool BrowserJavaAllowed { 
			[DebuggerStepThrough]
			get { return GetOption(OptionalFlags.AllowJavaInBrowser); }
			set 
			{
				SetOption(OptionalFlags.AllowJavaInBrowser, value);
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Sets/Get if ActiveX controls should be allowed to execute
		/// </summary>
		public bool BrowserActiveXAllowed { 
			[DebuggerStepThrough]
			get { return GetOption(OptionalFlags.AllowActiveXInBrowser); }
			set 
			{
				SetOption(OptionalFlags.AllowActiveXInBrowser, value);
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Sets/Get if background sounds are allowed to be played
		/// </summary>
		public bool BrowserBGSoundAllowed { 
			[DebuggerStepThrough]
			get { return GetOption(OptionalFlags.AllowBGSoundInBrowser); }
			set 
			{
				SetOption(OptionalFlags.AllowBGSoundInBrowser, value);
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Sets/Get if video can be played
		/// </summary>
		public bool BrowserVideoAllowed { 
			[DebuggerStepThrough]
			get { return GetOption(OptionalFlags.AllowVideoInBrowser); }
			set 
			{
				SetOption(OptionalFlags.AllowVideoInBrowser, value);
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Sets/Get if images should be loaded
		/// </summary>
		public bool BrowserImagesAllowed { 
			[DebuggerStepThrough]
			get { return GetOption(OptionalFlags.AllowImagesInBrowser); }
			set 
			{
				SetOption(OptionalFlags.AllowImagesInBrowser, value);
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Sets/Get the DisplayFeedAlertWindow enumeration value
		/// </summary>
		public DisplayFeedAlertWindow ShowAlertWindow { 
			[DebuggerStepThrough]
			get { return feedAlertWindow; }
			set 
			{
				feedAlertWindow = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Sets/Get if the system tray balloon tip should be displayed
		/// if new news items are received.
		/// </summary>
		public bool ShowNewItemsReceivedBalloon { 
			[DebuggerStepThrough]
			get { return GetOption(OptionalFlags.ShowNewItemsReceivedBalloon); }
			set 
			{
				SetOption(OptionalFlags.ShowNewItemsReceivedBalloon, value);
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Sets/Get if we build the relation cosmos (interlinkage of news items).
		/// </summary>
		public bool BuildRelationCosmos { 
			[DebuggerStepThrough]
			get { return true; /* we always want to do this given performance and usability improvements */ }
			set {
				/* do nothing */ 
			}
		}
		#endregion

		#region private OptionalFlags handling
		private bool GetOption(OptionalFlags flag)
		{
			return ((this.allOptionalFlags & flag) == flag);
		}

		private void SetOption(OptionalFlags flag, bool value)
		{
			if (value)
				this.allOptionalFlags |= flag;
			else
				this.allOptionalFlags = this.allOptionalFlags & ~flag;
		}
		#endregion

		#region ctor's
		public RssBanditPreferences()	{
			InitDefaults();
		}
		#endregion

		#region Init
		private void InitDefaults() {
			normalFont = FontColorHelper.DefaultNormalFont;
			unreadFont = FontColorHelper.DefaultUnreadFont;
			flagFont = FontColorHelper.DefaultHighlightFont;
			errorFont = FontColorHelper.DefaultFailureFont;
			referrerFont = FontColorHelper.DefaultReferenceFont;
			newCommentsFont = FontColorHelper.DefaultNewCommentsFont;

			numEnclosuresToDownloadOnNewFeed = FeedSource.DefaultNumEnclosuresToDownloadOnNewFeed;
			enclosureCacheSize = FeedSource.DefaultEnclosureCacheSize;

			// init default options to true:
			this.allOptionalFlags = DefaultOptionalFlags;
		}

		private static OptionalFlags DefaultOptionalFlags {
			get {
				OptionalFlags f = OptionalFlags.AllOff;
				f |= OptionalFlags.ByPassProxyOnLocal |
				//	OptionalFlags.ShowNewItemsReceivedBalloon |
					OptionalFlags.AllowImagesInBrowser |
					OptionalFlags.NewsItemOpenLinkInDetailWindow |
					OptionalFlags.ReUseFirstBrowserTab |
					OptionalFlags.AllowAppEventSounds  | 
                    OptionalFlags.BuildRelationCosmos| 
                    OptionalFlags.CreateSubfoldersForEnclosures|
                    OptionalFlags.RefreshFeedsOnStartup;
				return f;
			}
		}
		
		#endregion

		#region Serializing
		/// <summary>
		/// Initializes a new instance of the <see cref="RssBanditPreferences"/> class.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="context">The context.</param>
		protected RssBanditPreferences(SerializationInfo info, StreamingContext context) {
			
			InitDefaults();
			
			SerializationInfoReader reader = new SerializationInfoReader(info, context);

			int version = reader.Get("_PrefsVersion", 0);
			// new encryption key with version 21 and higher:
			EncryptionHelper.CompatibilityMode = (version <= 20);
			//bool xmlFormat = (version >= 20);

           this.allOptionalFlags = reader.Get("AllOptionalFlags", DefaultOptionalFlags);
			
			// all the following if (reader.Contains() calls are for migration from binary format
			// and because booleans are stored now in a flagged enum (OptionalFlags), that gets read once above.
			if (reader.Contains(nameof(UseProxy)))
				UseProxy = reader.Get(nameof(UseProxy), false);

			ProxyAddress = reader.Get(nameof(ProxyAddress), String.Empty);
			ProxyPort = reader.Get(nameof(ProxyPort), 8080);
			ProxyUser = EncryptionHelper.Decrypt(reader.Get(nameof(ProxyUser), String.Empty));
			ProxyPassword = EncryptionHelper.Decrypt(reader.Get(nameof(ProxyPassword), String.Empty));
			
			if (reader.Contains(nameof(BypassProxyOnLocal)))
				BypassProxyOnLocal = reader.Get(nameof(BypassProxyOnLocal), true);
			if (reader.Contains(nameof(ProxyCustomCredentials)))
				ProxyCustomCredentials = reader.Get(nameof(ProxyCustomCredentials), false);

			NewsItemStylesheetFile = reader.Get(nameof(NewsItemStylesheetFile), String.Empty);

			// see also version >= 16 below...
			// we still read them to enable migration
			// but newer formats do not store that anymore
			if (version < 18) {
				UserName = reader.Get(nameof(UserName), String.Empty);
				UserMailAddress = reader.Get(nameof(UserMailAddress), String.Empty);
				Referer = reader.Get(nameof(Referer), String.Empty);
			}

			HideToTrayAction = reader.Get(nameof(HideToTrayAction), HideToTray.OnMinimize);

			#region read Fonts

			if (reader.Contains("NormalFontString"))	// current
				NormalFont = reader.GetFont("NormalFontString", FontColorHelper.DefaultNormalFont);
			else	// older versions may contain:
				NormalFont = reader.Get("NormalFont",FontColorHelper.DefaultNormalFont);
			
			if (reader.Contains("UnreadFontString"))	// current
				UnreadFont = reader.GetFont("UnreadFontString", FontColorHelper.DefaultUnreadFont);
			else if (reader.Contains("HighlightFontString"))	// older than v1.5.0.8
				UnreadFont = reader.GetFont("HighlightFontString", FontColorHelper.DefaultUnreadFont);
			else	// older then v1.4.x:
				UnreadFont = reader.Get("HighlightFont", FontColorHelper.DefaultUnreadFont);

			if (reader.Contains("FlagFontString"))	// current
				FlagFont = reader.GetFont("FlagFontString", FontColorHelper.DefaultHighlightFont);
			else	// older versions may contain:
				FlagFont = reader.Get("FlagFont", FontColorHelper.DefaultHighlightFont);
			
			if (reader.Contains("ErrorFontString"))	// current
				ErrorFont = reader.GetFont("ErrorFontString", FontColorHelper.DefaultFailureFont);
			else	// older versions may contain:
				ErrorFont = reader.Get("ErrorFont", FontColorHelper.DefaultFailureFont);
			
			if (reader.Contains("ReferrerFontString"))	// current
				ReferrerFont = reader.GetFont("ReferrerFontString", FontColorHelper.DefaultReferenceFont);
			else if (reader.Contains("RefererFontString"))	
				ReferrerFont = reader.GetFont("RefererFontString", FontColorHelper.DefaultReferenceFont);
			else	// older versions may contain:
				ReferrerFont = reader.Get("RefererFont", FontColorHelper.DefaultReferenceFont);
			
			// new with 1.5.0.x:
			NewCommentsFont = reader.GetFont("NewCommentsFontString", FontColorHelper.DefaultNewCommentsFont);
			#endregion

			#region read colors

			NormalFontColor = reader.Get(nameof(NormalFontColor), FontColorHelper.DefaultNormalColor);
			if (reader.Contains(nameof(UnreadFontColor)))	// current
				UnreadFontColor = reader.Get(nameof(UnreadFontColor), FontColorHelper.DefaultUnreadColor);
			else	// older versions may contain the old key:
				UnreadFontColor = reader.Get("HighlightFontColor", FontColorHelper.DefaultUnreadColor);

			FlagFontColor = reader.Get(nameof(FlagFontColor), FontColorHelper.DefaultHighlightColor);
			ErrorFontColor = reader.Get(nameof(ErrorFontColor), FontColorHelper.DefaultFailureColor);

			if (reader.Contains(nameof(ReferrerFontColor)))	// current
				ReferrerFontColor = reader.Get(nameof(ReferrerFontColor), FontColorHelper.DefaultReferenceColor);
			else
				ReferrerFontColor = reader.Get("RefererFontColor", FontColorHelper.DefaultReferenceColor);
			
			// new with 1.5.0.x:
			NewCommentsFontColor = reader.Get(nameof(NewCommentsFontColor), FontColorHelper.DefaultNewCommentsColor);
			
			#endregion

			MaxItemAge = TimeSpan.FromTicks(reader.Get(nameof(MaxItemAge), TimeSpan.FromDays(90).Ticks));
			
			if (reader.Contains(nameof(UseRemoteStorage)))
				UseRemoteStorage = reader.Get(nameof(UseRemoteStorage), false);

			if (reader.Contains(nameof(RemoteStorageUserName)))	{
				RemoteStorageUserName = reader.Get(nameof(RemoteStorageUserName), String.Empty);
			} else {
				RemoteStorageUserName = EncryptionHelper.Decrypt(reader.Get("RemoteStorageUserNameCrypted", String.Empty));
			}
			if (reader.Contains(nameof(RemoteStoragePassword))) {
				RemoteStoragePassword = reader.Get(nameof(RemoteStoragePassword), String.Empty);
			} else {
				RemoteStoragePassword = EncryptionHelper.Decrypt(reader.Get("RemoteStoragePasswordCrypted", String.Empty));
			}

			RemoteStorageProtocol = reader.Get(nameof(RemoteStorageProtocol), RemoteStorageProtocolType.Unknown);
			RemoteStorageLocation = reader.Get(nameof(RemoteStorageLocation), String.Empty);
				// dasBlog_1_3 is not anymore supported:
			if (UseRemoteStorage && RemoteStorageProtocol == RemoteStorageProtocolType.dasBlog_1_3) {
				UseRemoteStorage = false;	
			}

			BrowserOnNewWindow = reader.Get(nameof(BrowserOnNewWindow), BrowserBehaviorOnNewWindow.OpenDefaultBrowser);
			BrowserCustomExecOnNewWindow = reader.Get(nameof(BrowserCustomExecOnNewWindow), String.Empty);

			if (reader.Contains(nameof(NewsItemOpenLinkInDetailWindow))) {
				NewsItemOpenLinkInDetailWindow = reader.Get(nameof(NewsItemOpenLinkInDetailWindow), true);
			}
			if (reader.Contains(nameof(UseIEProxySettings))) {
				UseIEProxySettings = reader.Get(nameof(UseIEProxySettings), false);
			}
			if (reader.Contains(nameof(FeedRefreshOnStartup))) {
				FeedRefreshOnStartup = reader.Get(nameof(FeedRefreshOnStartup), true);
			}
			if (reader.Contains(nameof(BrowserJavascriptAllowed))) {
				BrowserJavascriptAllowed = reader.Get(nameof(BrowserJavascriptAllowed), false);
			}
			if (reader.Contains(nameof(BrowserJavaAllowed))) {
				BrowserJavaAllowed = reader.Get(nameof(BrowserJavaAllowed), false);
			}
			if (reader.Contains(nameof(BrowserActiveXAllowed))) {
				BrowserActiveXAllowed = reader.Get(nameof(BrowserActiveXAllowed), false);
			}
			if (reader.Contains(nameof(BrowserBGSoundAllowed))) {
				BrowserBGSoundAllowed = reader.Get(nameof(BrowserBGSoundAllowed), false);
			}
			if (reader.Contains(nameof(BrowserVideoAllowed))) {
				BrowserVideoAllowed = reader.Get(nameof(BrowserVideoAllowed), false);
			}
			if (reader.Contains(nameof(BrowserImagesAllowed))) {
				BrowserImagesAllowed = reader.Get(nameof(BrowserImagesAllowed), true);
			}
			
			if (reader.Contains("ShowConfiguredAlertWindows")) {
				bool showConfiguredAlertWindows = reader.Get("ShowConfiguredAlertWindows", false);
				// migrate the old bool value to the new enum:
				if (showConfiguredAlertWindows) {
					ShowAlertWindow = DisplayFeedAlertWindow.AsConfiguredPerFeed;
				} else {
					ShowAlertWindow = DisplayFeedAlertWindow.None;
				}
			} else {
				ShowAlertWindow = reader.Get(nameof(ShowAlertWindow), DisplayFeedAlertWindow.AsConfiguredPerFeed);
			}

			if (reader.Contains(nameof(ShowNewItemsReceivedBalloon))) {
				ShowNewItemsReceivedBalloon = reader.Get(nameof(ShowNewItemsReceivedBalloon), false);
			}
			
			ProxyBypassList = reader.Get(nameof(ProxyBypassList), new string[]{});
			if (ProxyBypassList == null)
				ProxyBypassList = new string[]{};

			if (reader.Contains(nameof(MarkItemsReadOnExit))) {
				MarkItemsReadOnExit = reader.Get(nameof(MarkItemsReadOnExit), false);
			}

			UserIdentityForComments = reader.Get(nameof(UserIdentityForComments), String.Empty);
			
			if (reader.Contains(nameof(ReuseFirstBrowserTab))) {
				ReuseFirstBrowserTab = reader.Get(nameof(ReuseFirstBrowserTab), true);
			}

			this.NgosSyncToken = reader.Get(nameof(NgosSyncToken), String.Empty); 

			this.NumNewsItemsPerPage = reader.Get(nameof(NumNewsItemsPerPage), 10);

            this.ReadingPaneTextSize = reader.Get(nameof(ReadingPaneTextSize), TextSize.Medium);

			this.RefreshRate = reader.Get(nameof(RefreshRate), FeedSource.DefaultRefreshRate);
            this.EnclosureFolder = reader.Get(nameof(EnclosureFolder), String.Empty);
            this.EnclosureFolder = String.IsNullOrWhiteSpace(this.EnclosureFolder) 
                ? RssBanditApplication.GetDefaultEnclosuresPath()
                : this.EnclosureFolder; 
			this.NumEnclosuresToDownloadOnNewFeed = reader.Get(nameof(NumEnclosuresToDownloadOnNewFeed), FeedSource.DefaultNumEnclosuresToDownloadOnNewFeed);
			this.EnclosureCacheSize = reader.Get(nameof(EnclosureCacheSize), FeedSource.DefaultEnclosureCacheSize);
            this.PodcastFolder = reader.Get(nameof(PodcastFolder), String.Empty);
            this.PodcastFolder = String.IsNullOrWhiteSpace(this.PodcastFolder)
                ? RssBanditApplication.GetDefaultPodcastPath()
                : this.PodcastFolder; 
			this.PodcastFileExtensions = reader.Get(nameof(PodcastFileExtensions), RssBanditApplication.DefaultPodcastFileExts);
		}

		/// <summary>
		/// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/>
		/// with the data needed to serialize the target object.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
		/// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
		[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter=true),
		 SecurityPermissionAttribute(SecurityAction.LinkDemand)]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)	
		{
		
			info.AddValue("_PrefsVersion", 25);	// added refresh rate
			EncryptionHelper.CompatibilityMode = false;
			info.AddValue(nameof(ProxyAddress), ProxyAddress);
			info.AddValue(nameof(ProxyPort), ProxyPort);
			info.AddValue(nameof(ProxyUser), EncryptionHelper.Encrypt(ProxyUser));
			info.AddValue(nameof(ProxyPassword), EncryptionHelper.Encrypt(ProxyPassword));
			info.AddValue(nameof(ProxyBypassList), ProxyBypassList);
			info.AddValue(nameof(NewsItemStylesheetFile), NewsItemStylesheetFile);
			info.AddValue(nameof(HideToTrayAction), HideToTrayAction.ToString());
			info.AddValue(nameof(NormalFont)+"String", SerializationInfoReader.ConvertFont(NormalFont));
			info.AddValue(nameof(UnreadFont) + "String", SerializationInfoReader.ConvertFont(UnreadFont));
			info.AddValue(nameof(FlagFont)+"String", SerializationInfoReader.ConvertFont(FlagFont));
			info.AddValue(nameof(ErrorFont)+"String", SerializationInfoReader.ConvertFont(ErrorFont));
			info.AddValue(nameof(ReferrerFont)+"String", SerializationInfoReader.ConvertFont(ReferrerFont));
			info.AddValue(nameof(NewCommentsFont)+"String", SerializationInfoReader.ConvertFont(NewCommentsFont));
			info.AddValue(nameof(NormalFontColor), NormalFontColor);
			info.AddValue(nameof(UnreadFontColor), UnreadFontColor);
			info.AddValue(nameof(FlagFontColor), FlagFontColor);
			info.AddValue(nameof(ErrorFontColor), ErrorFontColor);
			info.AddValue(nameof(ReferrerFontColor), ReferrerFontColor);
			info.AddValue(nameof(NewCommentsFontColor), NewCommentsFontColor);
			info.AddValue(nameof(MaxItemAge), MaxItemAge.Ticks);
			info.AddValue(nameof(RemoteStorageUserName)+"Crypted", EncryptionHelper.Encrypt(RemoteStorageUserName));
			info.AddValue(nameof(RemoteStoragePassword)+"Crypted", EncryptionHelper.Encrypt(RemoteStoragePassword));
			info.AddValue(nameof(RemoteStorageProtocol), RemoteStorageProtocol.ToString());
			info.AddValue(nameof(RemoteStorageLocation), RemoteStorageLocation);
			info.AddValue(nameof(BrowserOnNewWindow), BrowserOnNewWindow.ToString());
			info.AddValue(nameof(BrowserCustomExecOnNewWindow), BrowserCustomExecOnNewWindow.ToString());
			info.AddValue(nameof(ShowAlertWindow), ShowAlertWindow.ToString());
			info.AddValue(nameof(UserIdentityForComments), UserIdentityForComments); 
			info.AddValue("AllOptionalFlags", this.allOptionalFlags.ToString());
			info.AddValue(nameof(NgosSyncToken), this.NgosSyncToken); 
			info.AddValue(nameof(NumNewsItemsPerPage), this.NumNewsItemsPerPage);
            info.AddValue(nameof(ReadingPaneTextSize), this.ReadingPaneTextSize.ToString());
			info.AddValue(nameof(RefreshRate), this.RefreshRate);
			info.AddValue(nameof(EnclosureFolder), this.EnclosureFolder);
			info.AddValue(nameof(NumEnclosuresToDownloadOnNewFeed), this.NumEnclosuresToDownloadOnNewFeed);
			info.AddValue(nameof(EnclosureCacheSize), this.EnclosureCacheSize);
			info.AddValue(nameof(PodcastFolder), this.PodcastFolder);
			info.AddValue(nameof(PodcastFileExtensions), this.PodcastFileExtensions);
		}
		#endregion

		
		#region helper classes
		private class EncryptionHelper {
			private static TripleDESCryptoServiceProvider _des;
			private static bool _compatibilityMode = false;

			private EncryptionHelper(){}

			static EncryptionHelper() {
				_des = new TripleDESCryptoServiceProvider();
				_des.Key = _calcHash();
				_des.Mode = CipherMode.ECB;
			}

			/// <summary>
			/// Just to enable read of old encrypted values by 
			/// providing the value 'true'.
			/// </summary>
			internal static bool CompatibilityMode { 
				get { return _compatibilityMode; }
				set {
					if (value != _compatibilityMode)
						_des.Key = _calcHash();
					_compatibilityMode = value;
				}
			}

			public static string Decrypt(string str) {
				byte[] base64;
				byte[] bytes;
				string ret;

				if (str == null)
					ret = null;
				else {
					if (str.Length == 0)
						ret = String.Empty;
					else {
						try {
							base64 = Convert.FromBase64String(str);
							bytes = _des.CreateDecryptor().TransformFinalBlock(base64, 0, base64.GetLength(0));
							ret = Encoding.Unicode.GetString(bytes);
						}
						catch (Exception e) {
							_log.Debug("Exception in Decrypt", e);
							ret = String.Empty;
						}
					}
				}
				return ret;
			}

			public static string Encrypt(string str) {
				byte[] inBytes;
				byte[] bytes;
				string ret;

				if (str == null)
					ret = null;
				else {
					if (str.Length == 0)
						ret = String.Empty;
					else {
						try {
							inBytes = Encoding.Unicode.GetBytes(str);
							bytes = _des.CreateEncryptor().TransformFinalBlock(inBytes, 0, inBytes.GetLength(0));
							ret = Convert.ToBase64String(bytes);
						}
						catch (Exception e) {
							_log.Debug("Exception in Encrypt", e);
							ret = String.Empty;
						}
					}
				}
				return ret;
			}

			private static byte[] _calcHash() 
			{
				// for FIPS compliance we just return the hash we formerly calculated.
				// This is for backward compatibility, so users do not loose all their
				// feed/feedsource/ftp/ etc. credentials...
				byte[] h = new byte[16];
				if (_compatibilityMode)
				{
					h[0] = 120;
					h[1] = 40;
					h[2] = 4;
					h[3] = 105;
					h[4] = 228;
					h[5] = 255;
					h[6] = 178;
					h[7] = 45;
					h[8] = 118;
					h[9] = 90;
					h[10] = 179;
					h[11] = 178;
					h[12] = 149;
					h[13] = 150;
					h[14] = 125;
					h[15] = 185;
				}
				else
				{
					h[0] = 33;
					h[1] = 97;
					h[2] = 12;
					h[3] = 205;
					h[4] = 28;
					h[5] = 181;
					h[6] = 25;
					h[7] = 20;
					h[8] = 55;
					h[9] = 214;
					h[10] = 222;
					h[11] = 35;
					h[12] = 111;
					h[13] = 239;
					h[14] = 96;
					h[15] = 42;
				}
				return h;

				//string salt = null;
				//if (_compatibilityMode) {
				//    // use the old days salt string.
				//    // this is not just a query: it will also create the folder :-(
				//    salt = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				//} else {
				//    // so here is a possibly better way to init the salt without
				//    // query the file system:
				//    salt = "B*A!N_D:I;T,P1E0P%P$E+R";
				//}

				//byte[] b = Encoding.Unicode.GetBytes(salt);
				//int bLen = b.GetLength(0);
				
				//// just to make the key somewhat "invisible" in Anakrino, we use the random class.
				//// the seed (a prime number) makes it repro
				//Random r = new Random(1500450271);	
				//// result array
				//byte[] res = new Byte[500];
				//int i = 0;
				
				//for (i = 0; i < bLen && i < 500; i++)
				//    res[i] = (byte)(b[i] ^ r.Next(30, 127));
				
				//// padding:
				//while (i < 500) {
				//    res[i] = (byte)r.Next(30, 127);
				//    i++;
				//}

				//MD5CryptoServiceProvider csp = new MD5CryptoServiceProvider();
				//byte[] cspr = csp.ComputeHash(res);
				//return cspr;
			}



		}
		
		/// <summary>
		/// Helps to deserialize the old RSSBandit.XXXTypes classes/types.
		/// It maps the type to the new namespace and same class.
		/// </summary>
		internal class DeserializationTypeBinder: SerializationBinder {
			
			//private static string assemblyRunning = Assembly.GetExecutingAssembly().FullName;
			// here are the enums moved from RssBandit assembly to AppServices:
            private static readonly List<string> movedTypes = new List<string>(
				new string[]{"RssBandit.HideToTray", 
								"RssBandit.AutoUpdateMode", 
								"RssBandit.RemoteStorageProtocolType", 
								"RssBandit.BrowserBehaviorOnNewWindow", 
								"RssBandit.DisplayFeedAlertWindow"});

			/// <summary>
			/// When overridden in a derived class, controls the binding of a serialized object to a type.
			/// </summary>
			/// <param name="assemblyName">Specifies the <see cref="T:System.Reflection.Assembly"></see> name of the serialized object.</param>
			/// <param name="typeName">Specifies the <see cref="T:System.Type"></see> name of the serialized object.</param>
			/// <returns>
			/// The type of the object the formatter creates a new instance of.
			/// </returns>
			public override Type BindToType(string assemblyName, string typeName) 
			{
				Type typeToDeserialize = null;

				// For each assemblyName/typeName that you wish to deserialize 
				// to a different type, set typeToDeserialize to the desired type
				
				if (movedTypes.Contains(typeName)) {
					// moved types (from RssBandit to AppServices assembly):
					int index = assemblyName.IndexOf("AppServices");
					if (index < 0) {
						typeToDeserialize = Type.GetType(String.Format("{0}, {1}", 
							typeName, "RssBandit.AppServices"));
					}
					else if (index > 0)
					{ 	// version incorrect types (AppServices assembly):
						typeToDeserialize = Type.GetType(String.Format("{0}, {1}",
							typeName, "RssBandit.AppServices"));
					}
				}

				// very old: namespace name changed (now mixed case)
				string typeVer1 = "RSSBandit.";	

				if (typeName.IndexOf(typeVer1) == 0 ) {
					// old namespace found
					typeName = typeName.Replace(typeVer1, "RssBandit.");
					typeToDeserialize = Type.GetType(String.Format("{0}, {1}", 
						typeName, assemblyName));
				}

				// old file with strong named assembly refs (e.g. "RssBandit.AppServices, Version=1.6.0.3, Culture=neutral, PublicKeyToken=39cb28311174616c")
				int simpleAssemblyNameEnd = assemblyName.IndexOf(", Version=");
				if (typeToDeserialize == null && simpleAssemblyNameEnd >= 0)
				{
					typeToDeserialize = Type.GetType(String.Format("{0}, {1}",
							typeName, assemblyName.Substring(0, simpleAssemblyNameEnd)), false);
					
				}

				// in case System.Drawing assemlby was not yet loaded:
				if (typeToDeserialize == null && typeName == "System.Drawing.Color")
					typeToDeserialize = typeof(Color);
				return typeToDeserialize;
			}

		}
		#endregion
	}
}
