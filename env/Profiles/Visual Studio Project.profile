{  
    "Name":"Visual Studio Project",
    "CreateDate":"2017-01-18",
    "ModifyDate":"2017-01-18",
    "IgnoredDirectories":[  
        "^.svn$",
        "^.vs$",
        "^bin$",
        "^obj$",
        "^tmp$",
        "^packages$"
    ],
    "IgnoredFiles":[  
        "\\.tmp$",
        "\\.DotSettings.user$",
		"\\.(7z|zip|rar)$"
    ],
    "FileNameBuilder":"AddDateTime",
    "Password":null,
    "UsePlainTextPasswords":false,
    "ProviderName":"SharpZip",
	"UseDefaultActionPrecedence": "true",
	"BreakActionsIfError": "true",
	"AfterFinishedActions":[  
		
                {
                        "Name": "SetClipboard"
                },
                {
                        "Name": "SendToGoogleDrive",
                        "Context": {
                                "SecretKeyFilePath": "GoogleDriveCredentials.json",
                                "DestinationFolderId": "1fICLyjLPBSm3phufBdAnjzKcG29twQ81",
                        }
                }
    ]
}