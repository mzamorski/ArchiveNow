{
	"Name": "Project 'EmbroideryManager'",
	"CreateDate": "2019-04-12",
	"ModifyDate": "2019-04-12",
	"IgnoredDirectories": [
		"^.(svn|git)$",
        "^.vs$",
        "^obj$",
        "^packages$",
		"^backup$",
		"^bin$",
		"^tmp$",
		"^versions$",
		"^changes$",
		"^___$",
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