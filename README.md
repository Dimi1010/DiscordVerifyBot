# DiscordVerifyBot
A custom discord bot for granting/removing roles.

##	Access Levels:
1. 	L0 - No special access to commands.

	Default access level.
1. 	L1 - Access to basic verification commands.

	Manually assigned to users by L3 and above.
1. 	L2 - Access to verification approval commands.

	Manually assigned to users by L3 and above.
1.	L3 - Access to commands editing server wide bot actions.

	Automatically determined from Discord Manage Roles permission.

1.	L4 - Bot Owner only
	
##	Commands:

All commands begin with a prefix. ( default prefix is -- )

Possible parameters are surrounded in square brackets []
1.	Public commands:
	1.	Pending

		Shows information for all pending request forms for the server.

1.	Verification commands:

	(Require at least L1 access)
	1.	Verify [\<user mention> or \<user name> or \<snowflake_userID>]

		Submits a pending verification form for approval.

		If executed by a user with L2 access or above the form is automatically approved and processed immediatly.

1.	Approval commands:

	(Require at least L2 access)
	1.	Approve [\<user mention> or \<user name> or \<snowflake_userID>]

		Approved a request for verification for the specified user.

	1.	Approve-All

		Approved all pending verification forms for the current server.

	1.	Deny [\<user mention> or \<user name> or \<snowflake_userID>]

		Denies a request for verification for the specified user.

1.	User Permission management:

	(Require at least L3 access)
	1.	Add-Verifier [\<user mention> or \<user name> or \<snowflake_userID>]

		Grants the specified user L1 access.

	1.	Remove-Verifier [\<user mention> or \<user name> or \<snowflake_userID>]

		Revokes the specified user's L1 access.

	1.	Add-Approver [\<user mention> or \<user name> or \<snowflake_userID>]

		Grants the specified user L2 access.

	1.	Remove-Approver [\<user mention> or \<user name> or \<snowflake_userID>]

		Revokes the specified user's L2 access.

1.	Verification processing parameters:

	(Require at least L3 access)
	1.	Add-Role [\<add> or \<remove>] [\<role mention> or \<snowflake_roleID>]

		Adds role to be added or removed to the user when verification is processed.

	1.	Remove-Role [\<role mention> or \<snowflake_roleID>]

		Removes the role from the list of roles that are modified when verification is processed.
