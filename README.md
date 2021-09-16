</br>
 
# &nbsp;Firenet
 
Firestore orm for .net applications. A simple easy way to manipulate firestore database and keeping the same domain driven design, similiar the entityframework for be easy to work. 

</br>

## Installation
Download by dotnet cli:  

```   
  Install-Package Firenet -Version 1.2.0
```
## Release notes
- Inclusion of default converter for guid and datetime types into domain model, now is possible use freely this types.
- Inclusion of OnConfiguring method into the Firecontext optional for set your preferences.
- Inclusion of UseJsonCredentialsFile method into the FireOption type for set the filename of credentials that is the same folder where the assemblies from application is running.
- Fixed the select method into the asQueryable, working better now.

## How to use
Put the data annotations in the class model and properties:  
![image](https://user-images.githubusercontent.com/30809620/120728408-3a39f900-c4b3-11eb-93c9-05eb8607b59d.png)

Create a class that implement 'FireContext' and create the fireCollections that represents the models that you want store:  
![image](https://user-images.githubusercontent.com/30809620/133532334-30caada3-b63e-48f2-a7f8-7c6a45de5a89.png)  
Or:  
![image](https://user-images.githubusercontent.com/30809620/133536201-7821ae8e-7b2d-4c06-b384-c1fa121b91e1.png)  

You can put data annotation 'CollectionName' for informate the name of collection. If it doesn't have collection name data annotation, the name of collection will be the name of object IFireCollection ('Users' in this example).  
![image](https://user-images.githubusercontent.com/30809620/120899679-4df67400-c607-11eb-932b-b6588bc39002.png)

Now it's possible configure inside the firecontext:  
![image](https://user-images.githubusercontent.com/30809620/133533668-e07e8d47-ba32-4fc6-9010-b5557eb57758.png)  
Or:  
![image](https://user-images.githubusercontent.com/30809620/133532966-379913f6-c436-4ce3-908a-3f36ad00cf7a.png)  

Add on startup configureServices:  
![image](https://user-images.githubusercontent.com/30809620/120727866-feeafa80-c4b1-11eb-8e81-b4feab63224f.png) 

Or in console application, instance:  
![image](https://user-images.githubusercontent.com/30809620/133533547-0e56eadd-8af4-4859-a26d-1a68ab32ad35.png)  

