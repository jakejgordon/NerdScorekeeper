/** This script clears out any sensitive information from the database*/
BEGIN TRANSACTION
DELETE FROM [AspNetUserLogins];
UPDATE AspnetUsers SET Email = 'fake@mailinator.com', PasswordHash = 'fake';
UPDATE GamingGroupInvitation SET InviteeEmail = 'fake@mailinator.com';
DELETE FROM UserDeviceAuthToken;
ROLLBACK TRANSACTION --uncomment to sanitize away