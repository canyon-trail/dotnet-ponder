module Ponder.SlnTypes

type SlnProject = {
    Name: string;
    Path: string;
}

type SlnFile = {
    Projects: SlnProject list
}

let EmptySln = {
    Projects = [];
}

type Project = {
    Name: string
    Path: string
    References: List<string>
    IsTestProject: bool
}
