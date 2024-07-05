{
	"Name": "Project 'ArchiveNow'",
	"CreateDate": "2023-12-31",
	"ModifyDate": "2023-12-31",
	"IgnoredDirectories": [
		"^.(svn|git)$",
        "^.vs$",
        "^obj$",
        "^packages$",
		"^bin$",
		"^tmp$",
	],
	"IgnoredFiles": [
        "\\.tmp$",
        "\\.DotSettings.user$",
		"\\.(7z|zip|rar)$"	
	],
	"FileNameBuilder": "AddDateTime",
	"Password": "ronin.135",
	"UsePlainTextPasswords": true,
	"ProviderName": "SevenZip",
	"UseDefaultActionPrecedence": "true",
	"AfterFinishedActions": [{
			"Name": "SetClipboard"
		}
	]
}