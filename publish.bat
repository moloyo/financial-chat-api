dotnet publish -c Release .\FinancialChat.sln
cd FinancialChat\bin\Release\net5.0\publish
heroku login
heroku container:login
heroku container:push web
heroku container:release web