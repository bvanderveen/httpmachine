#I "lib/FAKE"
#r "FakeLib.dll"
open Fake 
open Fake.MSBuild

(* properties *)
let projectName = "HttpMachine"
let version = "1.0.0-pre"  
let projectSummary = "HttpMachine is a .NET HTTP request parser."
let projectDescription = "HttpMachine is a .NET HTTP request parser."
let authors = ["Benjamin van der Veen"]
let mail = "b@bvanderveen.com"
let homepage = "https://github.com/bvanderveen/httpmachine"

(* Directories *)
let buildDir = "./build/"
let docsDir = "./docs/" 
let deployDir = "./deploy/"
let testDir = "./test/"
let nugetDir = "./nuget/" 

(* Tools *)
let nunitPath = "./packages/NUnit.2.5.9.10348/Tools"
let nunitOutput = testDir + "TestResults.xml"
let zipFileName = deployDir + sprintf "%s-%s.zip" projectName version

(* files *)
let appReferences =
    !+ @"src\**\*.csproj" 
      ++ @"src\**\*.fsproj"
      -- "**\*_Spliced*" 
        |> Scan

let filesToZip =
  !+ (buildDir + "/**/*.*")     
      -- "*.zip"
      |> Scan      

(* Targets *)
Target? Clean <-
    fun _ -> CleanDirs [buildDir; testDir; deployDir; docsDir; nugetDir]

Target? BuildApp <-
    fun _ -> 
        if not isLocalBuild then
          AssemblyInfo 
           (fun p -> 
              {p with
                 CodeLanguage = FSharp;
                 AssemblyVersion = buildVersion;
                 AssemblyTitle = projectName;
                 AssemblyDescription = projectDescription;
                 Guid = "1e95a279-c2a9-498b-bc72-6e7a0d6854ce";
                 OutputFileName = "./src/AssemblyInfo.fs"})

        appReferences
          |> MSBuildRelease buildDir "Build"
          |> Log "AppBuild-Output: "

Target? BuildTest <-
    fun _ -> 
        appReferences
          |> MSBuildDebug testDir "Build"
          |> Log "TestBuild-Output: "

Target? Test <-
    fun _ ->
        !+ (testDir + "/*.dll")
          |> Scan
          |> NUnit (fun p -> 
                      {p with 
                         ToolPath = nunitPath; 
                         DisableShadowCopy = true; 
                         OutputFile = nunitOutput}) 

Target? GenerateDocumentation <-
    fun _ ->
      !+ (buildDir + "Cashel.dll")      
        |> Scan
        |> Docu (fun p ->
            {p with
               ToolPath = "./lib/FAKE/docu.exe"
               TemplatesPath = "./lib/FAKE/templates"
               OutputPath = docsDir })

Target? CopyLicense <-
    fun _ ->
        [ "LICENSE.txt" ] |> CopyTo buildDir

Target? BuildZip <-
    fun _ -> Zip buildDir zipFileName filesToZip

Target? ZipDocumentation <-
    fun _ ->    
        let docFiles = 
          !+ (docsDir + "/**/*.*")
            |> Scan
        let zipFileName = deployDir + sprintf "Documentation-%s.zip" version
        Zip docsDir zipFileName docFiles

Target? CreateNuGet <-
    fun _ ->
        let nugetDocsDir = nugetDir @@ "docs/"
        let nugetToolsDir = nugetDir @@ "lib/"
        
        XCopy docsDir nugetDocsDir
        XCopy buildDir nugetToolsDir
        
        NuGet (fun p ->
            {p with
                Authors = authors
                Project = projectName
                Description = projectDescription
                OutputPath = nugetDir }) "Cashel.nuspec"

Target? Default <- DoNothing
Target? Deploy <- DoNothing

// Dependencies
For? BuildApp <- Dependency? Clean
For? Test <- Dependency? BuildApp |> And? BuildTest
For? GenerateDocumentation <- Dependency? BuildApp
For? ZipDocumentation <- Dependency? GenerateDocumentation
For? BuildZip <- Dependency? BuildApp |> And? CopyLicense
For? CreateNuGet <- Dependency? Test |> And? BuildZip |> And? ZipDocumentation
For? Deploy <- Dependency? Test |> And? BuildZip |> And? ZipDocumentation
For? Default <- Dependency? Deploy

// start build
Run? Default
