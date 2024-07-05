{
	"Name": "Visual Studio Project",
	"CreateDate": "2017-01-18",
	"ModifyDate": "2017-01-18",
	"IgnoredDirectories": [
		"^.svn$",
		"^.vs$",
		"^bin$",
		"^obj$",
		"^tmp$",
		"^packages$"
	],
	"IgnoredFiles": [
		"\\.tmp$",
		"\\.DotSettings.user$",
		"\\.(7z|zip|rar)$"
	],
	"FileNameBuilder": "AddDateTime",
	"Password": null,
	"UsePlainTextPasswords": false,
	"ProviderName": "SevenZip",
	"UseDefaultActionPrecedence": "true",
	"AfterFinishedActions": [{
			"Name": "SetClipboard"
		}
	]
}