# Ovidja Database "API"
This is a collection of PHP files that the Android application uses to query
my Amazon RDS database instance. It's far from secure (vulnerable to SQL
injection, non-encrypted, etc.) and it's definitely not best practices, but I
didn't have weeks to get a REST API up and running. Instead, I came up with
this sad collection of PHP files hosted on my university public drive
that queries the database and returns the data as a JSON, which the application
then uses. It's rough, but it works. DO NOT USE THIS IN A PRODUCTION
ENVIRONMENT.
