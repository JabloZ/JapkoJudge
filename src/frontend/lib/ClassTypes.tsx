export type Challenge = {
  id: number;
  title: string;
  difficulty: number;
  description: string;
  verified: boolean;
  username: string;
  viewerOwner: boolean;
};
export type Manifest={
    id: number;
    challengeId: number;
    languageId: number;
    startCode:string;
    testfilePath:string;
    languageName:string;
    authorId:number;
};
export type Submission={
    id: number;
    ownerId:number;
    manifestId: number;
    code:string;
    status:string;
    message:string;
    memoryUsed:number;
    executionTime:number;
    challengeId:number;
    challengeTitle:string;
};
export type Profile={
    points:number;
    challenges:number;
}
export type Rank={
    username:string,
    challengeCount:number,
    score:number
}
export type Language={
    id:number,
    name:string,
    extension:string
}