// 
// GlyphNames.cs
//  
// Author:
//       Junichi OKADOME (tome@tomesoft.net)
// 
// Copyright 2012-2013 tomesoft.net
//	
//	Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//		
//		http://www.apache.org/licenses/LICENSE-2.0
//		
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Text;

namespace FontAwesome
{
    // Name constants for each glyph
    // To maintain this list, around the end of the font-awesome.scss should be referred to.
    // Unicode Private Use Area (PUA)
	public static class GlyphNames
	{
		public const string Glass            = "glass";            // "uniF000";
		public const string Music            = "music";            // "uniF001";
		public const string Search           = "search";           // "uniF002";
		public const string Envelope         = "envelope";         // "uniF003";
		public const string Heart            = "heart";            // "uniF004";
		public const string Star             = "star";             // "uniF005";
		public const string StarEmpty        = "star_empty";       // "uniF006";
		public const string User             = "user";             // "uniF007";
		public const string Film             = "film";             // "uniF008";
		public const string THLarge          = "th_large";         // "uniF009";
		public const string TH               = "th";               // "uniF00A";
		public const string THList           = "th_list";          // "uniF00B";
		public const string OK               = "ok";               // "uniF00C";
		public const string Remove           = "remove";           // "uniF00D";
		public const string ZoomIn           = "zoom_in";          // "uniF00E";

		public const string ZoomOut          = "zoom_out";         // "uniF010";
		public const string Off              = "off";              // "uniF011";
		public const string Signal           = "signal";           // "uniF012";
		public const string Cog              = "cog";              // "uniF013";
		public const string Trash            = "trash";            // "uniF014";
		public const string Home             = "home";             // "uniF015";
		public const string File             = "file";             // "uniF016";
		public const string Time             = "time";             // "uniF017";
		public const string Road             = "road";             // "uniF018";
		public const string DownloadAlt      = "download_alt";     // "uniF019";
		public const string Download         = "download";         // "uniF01A";
		public const string Upload           = "upload";           // "uniF01B";
		public const string Inbox            = "inbox";            // "uniF01C";
		public const string PlayCircle       = "play_circle";      // "uniF01D";
		public const string Repeat           = "repeat";           // "uniF01E";

                   // uniF020 skipped
		public const string Refresh          = "refresh";          // "uniF021";
		public const string ListAlt          = "list_alt";         // "uniF022";
		public const string Lock             = "lock";             // "uniF023";
		public const string Flag             = "flag";             // "uniF024";
		public const string Headphones       = "headphones";       // "uniF025";
		public const string VolumeOff        = "volume_off";       // "uniF026";
		public const string VolumeDown       = "volume_down";      // "uniF027";
		public const string VolumeUp         = "volume_up";        // "uniF028";
		public const string QRCode           = "qrcode";           // "uniF029";
		public const string BarCode          = "barcode";          // "uniF02A";
		public const string Tag              = "tag";              // "uniF02B";
		public const string Tags             = "tags";             // "uniF02C";
		public const string Book             = "book";             // "uniF02D";
		public const string Bookmark         = "bookmark";         // "uniF02E";
		public const string Print            = "print";            // "uniF02F";

		public const string Camera           = "camera";           // "uniF030";
		public const string Font             = "font";             // "uniF031";
		public const string Bold             = "bold";             // "uniF032";
		public const string Italic           = "italic";           // "uniF033";
		public const string TextHeight       = "text_height";      // "uniF034";
		public const string TextWidth        = "text_width";       // "uniF035";
		public const string AlignLeft        = "align_left";       // "uniF036";
		public const string AlignCenter      = "align_center";     // "uniF037";
		public const string AlignRight       = "align_right";      // "uniF038";
		public const string AlignJustify     = "align_justify";    // "uniF039";
		public const string List             = "list";             // "uniF03A";
		public const string IndentLeft       = "indent_left";      // "uniF03B";
		public const string IndentRight      = "indent_right";     // "uniF03C";
		public const string FacetimeVideo    = "facetime_video";   // "uniF03D";
		public const string Picture          = "picture";          // "uniF03E";

		public const string Pencil           = "pencil";           // "uniF040";
		public const string MapMarker        = "map_marker";       // "uniF041";
		public const string Adjust           = "adjust";           // "uniF042";
		public const string Tint             = "tint";             // "uniF043";
		public const string Edit             = "edit";             // "uniF044";
		public const string Share            = "share";            // "uniF045";
		public const string Check            = "check";            // "uniF046";
		public const string Move             = "move";             // "uniF047";
		public const string StepBackward     = "step_backward";    // "uniF048";
		public const string FastBackward     = "fast_backward";    // "uniF049";
		public const string Backward         = "backward";         // "uniF04A";
		public const string Play             = "play";             // "uniF04B";
		public const string Pause            = "pause";            // "uniF04C";
		public const string Stop             = "stop";             // "uniF04D";
		public const string Forward          = "forward";          // "uniF04E";

		public const string FastForward      = "fast_forward";     // "uniF050";
		public const string StepForward      = "step_forward";     // "uniF051";
		public const string Eject            = "eject";            // "uniF052";
		public const string ChevronLeft      = "chevron_left";     // "uniF053";
		public const string ChevronRight     = "chevron_right";    // "uniF054";
		public const string PlusSign         = "plus_sign";        // "uniF055";
		public const string MinusSign        = "minus_sign";       // "uniF056";
		public const string RemoveSign       = "remove_sign";      // "uniF057";
		public const string OKSign           = "ok_sign";          // "uniF058";
		public const string QuestionSign     = "question_sign";    // "uniF059";
		public const string InfoSign         = "info_sign";        // "uniF05A";
		public const string Screenshot       = "screenshot";       // "uniF05B";
		public const string RemoveCircle     = "remove_circle";    // "uniF05C";
		public const string OKCircle         = "ok_circle";        // "uniF05D";
		public const string BanCircle        = "ban_circle";       // "uniF05E";

		public const string ArrowLeft        = "arrow_left";       // "uniF060";
		public const string ArrowRight       = "arrow_right";      // "uniF061";
		public const string ArrowUp          = "arrow_up";         // "uniF062";
		public const string ArrowDown        = "arrow_down";       // "uniF063";
		public const string ShareAlt         = "share_alt";        // "uniF064";
		public const string ResizeFull       = "resize_full";      // "uniF065";
		public const string ResizeSmall      = "resize_small";     // "uniF066";
		public const string Plus             = "plus";             // "uniF067";
		public const string Minus            = "minus";            // "uniF068";
		public const string Asterisk         = "asterisk";         // "uniF069";
		public const string ExclamationSign  = "exclamation_sign"; // "uniF06A";
		public const string Gift             = "gift";             // "uniF06B";
		public const string Leaf             = "leaf";             // "uniF06C";
		public const string Fire             = "fire";             // "uniF06D";
		public const string EyeOpen          = "eye_open";         // "uniF06E";

		public const string EyeClose         = "eye_close";        // "uniF070";
		public const string WarningSign      = "warning_sign";     // "uniF071";
		public const string Plane            = "plane";            // "uniF072";
		public const string Calendar         = "calendar";         // "uniF073";
		public const string Random           = "random";           // "uniF074";
		public const string Comment          = "comment";          // "uniF075";
		public const string Magnet           = "magnet";           // "uniF076";
		public const string ChevronUp        = "chevron_up";       // "uniF077";
		public const string ChevronDown      = "chevron_down";     // "uniF078";
		public const string Retweet          = "retweet";          // "uniF079";
		public const string ShoppingCart     = "shopping_cart";    // "uniF07A";
		public const string FolderClose      = "folder_close";     // "uniF07B";
		public const string FolderOpen       = "folder_open";      // "uniF07C";
		public const string ResizeVertical   = "resize_vertical";  // "uniF07D";
		public const string ResizeHorizontal = "resize_horizontal"; // "uniF07E";

		public const string BarChart         = "bar_chart";        // "uniF080";
		public const string TwitterSign      = "twitter_sign";     // "uniF081";
		public const string FacebookSign     = "facebook_sign";    // "uniF082";
		public const string CameraRetro      = "camera_retro";     // "uniF083";
		public const string Key              = "key";              // "uniF084";
		public const string Cogs             = "cogs";             // "uniF085";
		public const string Comments         = "comments";         // "uniF086";
		public const string ThumbsUp         = "thumbs_up";        // "uniF087";
		public const string ThumbsDown       = "thumbs_down";      // "uniF088";
		public const string StarHalf         = "star_half";        // "uniF089";
		public const string HeartEmpty       = "heart_empty";      // "uniF08A";
		public const string Signout          = "signout";          // "uniF08B";
		public const string LinkedinSign     = "linkedin_sign";    // "uniF08C";
		public const string Pushpin          = "pushpin";          // "uniF08D";
		public const string ExternalLink     = "external_link";    // "uniF08E";

		public const string Signin           = "signin";           // "uniF090";
		public const string Trophy           = "trophy";           // "uniF091";
		public const string GithubSign       = "github_sign";      // "uniF092";
		public const string UploadAlt        = "upload_alt";       // "uniF093";
		public const string Lemon            = "lemon";            // "uniF094";
		public const string Phone            = "phone";            // "uniF095";
		public const string CheckEmpty       = "check_empty";      // "uniF096";
		public const string BookmarkEmpty    = "bookmark_empty";   // "uniF097";
		public const string PhoneSign        = "phone_sign";       // "uniF098";
		public const string Twitter          = "twitter";          // "uniF099";
		public const string Facebook         = "facebook";         // "uniF09A";
		public const string Github           = "github";           // "uniF09B";
		public const string Unlock           = "unlock";           // "uniF09C";
		public const string CreditCard       = "credit_card";      // "uniF09D";
		public const string Rss              = "rss";              // "uniF09E";

		public const string Hdd              = "hdd";              // "uniF0A0";
		public const string Bullhorn         = "bullhorn";         // "uniF0A1";
		public const string Bell             = "bell";             // "uniF0A2";
		public const string Certificate      = "certificate";      // "uniF0A3";
		public const string HandRight        = "hand_right";       // "uniF0A4";
		public const string HandLeft         = "hand_left";        // "uniF0A5";
		public const string HandUp           = "hand_up";          // "uniF0A6";
		public const string HandDown         = "hand_down";        // "uniF0A7";
		public const string CircleArrowLeft  = "circle_arrow_left"; // "uniF0A8";
		public const string CircleArrowRight = "circle_arrow_right"; // "uniF0A9";
		public const string CircleArrowUp    = "circle_arrow_up";  // "uniF0AA";
		public const string CircleArrowDown  = "circle_arrow_down"; // "uniF0AB";
		public const string Globe            = "globe";            // "uniF0AC";
		public const string Wrench           = "wrench";           // "uniF0AD";
		public const string Tasks            = "tasks";            // "uniF0AE";
        
		public const string Filter           = "filter";           // "uniF0B0";
		public const string Briefcase        = "briefcase";        // "uniF0B1";
		public const string Fullscreen       = "fullscreen";       // "uniF0B2";

		public const string Group            = "group";            // "uniF0C0";
		public const string Link             = "link";             // "uniF0C1";
		public const string Cloud            = "cloud";            // "uniF0C2";
		public const string Beaker           = "beaker";           // "uniF0C3";
		public const string Cut              = "cut";              // "uniF0C4";
		public const string Copy             = "copy";             // "uniF0C5";
		public const string PaperClip        = "paper_clip";       // "uniF0C6";
		public const string Save             = "save";             // "uniF0C7";
		public const string SignBlank        = "sign_blank";       // "uniF0C8";
		public const string Reorder          = "reorder";          // "uniF0C9";
		public const string ListUL           = "ul";               // "uniF0CA";
		public const string ListOL           = "ol";               // "uniF0CB";
		public const string Strikethrough    = "strikethrough";    // "uniF0CC";
		public const string Underline        = "underline";        // "uniF0CD";
		public const string Table            = "table";            // "uniF0CE";

		public const string Magic            = "magic";            // "uniF0D0";
		public const string Truck            = "truck";            // "uniF0D1";
		public const string Pinterest        = "pinterest";        // "uniF0D2";
		public const string PinterestSign    = "pinterest_sign";   // "uniF0D3";
		public const string GooglePlusSign   = "google_plus_sign"; // "uniF0D4";
		public const string GooglePlus       = "google_plus";      // "uniF0D5";
		public const string Money            = "money";            // "uniF0D6";
		public const string CaretDown        = "caret_down";       // "uniF0D7";
		public const string CaretUp          = "caret_up";         // "uniF0D8";
		public const string CaretLeft        = "caret_left";       // "uniF0D9";
		public const string CaretRight       = "caret_right";      // "uniF0DA";
		public const string Columns          = "columns";          // "uniF0DB";
		public const string Sort             = "sort";             // "uniF0DC";
		public const string SortDown         = "sort_down";        // "uniF0DD";
		public const string SortUp           = "sort_up";          // "uniF0DE";
        
		public const string EnvelopeAlt      = "envelope_alt";     // "uniF0E0";
		public const string Linkedin         = "linkedin";         // "uniF0E1";
		public const string Undo             = "undo";             // "uniF0E2";
		public const string Legal            = "legal";            // "uniF0E3";
		public const string Dashboard        = "dashboard";        // "uniF0E4";
		public const string CommentAlt       = "comment_alt";      // "uniF0E5";
		public const string CommentsAlt      = "comments_alt";     // "uniF0E6";
		public const string Bolt             = "bolt";             // "uniF0E7";
		public const string Sitemap          = "sitemap";          // "uniF0E8";
		public const string Umbrella         = "umbrella";         // "uniF0E9";
		public const string Paste            = "paste";            // "uniF0EA";
		public const string LightBulb        = "light_bulb";       // "uniF0EB";
		public const string Exchange         = "exchange";         // "uniF0EC";
		public const string CloudDownload    = "cloud_download";   // "uniF0ED";
		public const string CloudUpload      = "cloud_upload";     // "uniF0EE";

		public const string UserMD           = "user_md";          // "uniF0F0";
		public const string Stethoscope      = "stethoscope";      // "uniF0F1";
		public const string Suitcase         = "suitcase";         // "uniF0F2";
		public const string BellAlt          = "bell_alt";         // "uniF0F3";
		public const string Coffee           = "coffee";           // "uniF0F4";
		public const string Food             = "food";             // "uniF0F5";
		public const string FileAlt          = "file_alt";         // "uniF0F6";
		public const string Building         = "building";         // "uniF0F7";
		public const string Hospital         = "hospital";         // "uniF0F8";
		public const string Ambulance        = "ambulance";        // "uniF0F9";
		public const string Medkit           = "medkit";           // "uniF0FA";
		public const string FighterJet       = "fighter_jet";      // "uniF0FB";
		public const string Beer             = "beer";             // "uniF0FC";
		public const string HSign            = "h_sign";           // "uniF0FD";
		public const string PlusSignAlt      = "f0fe";             // "uniF0FE";

		public const string DoubleAngleLeft  = "double_angle_left"; // "uniF100";
		public const string DoubleAngleRight = "double_angle_right"; // "uniF101";
		public const string DoubleAngleUp    = "double_angle_up";  // "uniF102";
		public const string DoubleAngleDown  = "double_angle_down"; // "uniF103";
		public const string AngleLeft        = "angle_left";       // "uniF104";
		public const string AngleRight       = "angle_right";      // "uniF105";
		public const string AngleUp          = "angle_up";         // "uniF106";
		public const string AngleDown        = "angle_down";       // "uniF107";
		public const string Desktop          = "desktop";          // "uniF108";
		public const string Laptop           = "laptop";           // "uniF109";
		public const string Tablet           = "tablet";           // "uniF10A";
		public const string MobilePhone      = "mobile_phone";     // "uniF10B";
		public const string CircleBlank      = "circle_blank";     // "uniF10C";
		public const string QuoteLeft        = "quote_left";       // "uniF10D";
		public const string QuoteRight       = "quote_right";      // "uniF10E";

		public const string Spinner          = "spinner";          // "uniF110";
		public const string Circle           = "circle";           // "uniF111";
		public const string Reply            = "reply";            // "uniF112";
		public const string GithubAlt        = "github_alt";       // "uniF113";
		public const string FolderCloseAlt   = "folder_close_alt"; // "uniF114";
		public const string FolderOpenAlt    = "folder_open_alt";  // "uniF115";
    }
}
