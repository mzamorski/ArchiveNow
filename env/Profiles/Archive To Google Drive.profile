{  
    "Name":"Archive To Google Drive",
    "CreateDate":"2018-06-01",
    "ModifyDate":"2018-06-01",
    "IgnoredDirectories": [
		"^.svn$",
		"^.vs$",
		"^bin$",
		"^obj$",
		"^tmp$",
		"^packages$"
	],
    "IgnoredFiles":[  
        "\\.tmp$",
		"\\.7z$",
    ],
    "FileNameBuilder":"AddDateTime",
    "Password":null,
    "IsPasswordEncrypted":false,
    "ProviderName":"SharpZip",
	"UseDefaultActionPrecedence": "true",
	"AfterFinishedActions":[  
		{
			"Name": "UploadToGoogleDrive",
			"Context": {
				"SecretKeyFilePath": "GoogleDriveCredentials.json",
				"DestinationFolderId": "1fICLyjLPBSm3phufBdAnjzKcG29twQ81",
			}
		},
		{
			"Name": "SetClipboard",
		}
    ]
}