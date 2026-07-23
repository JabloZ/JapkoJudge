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
