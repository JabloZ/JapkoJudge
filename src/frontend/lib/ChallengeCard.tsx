import Link from "next/link";
export type Challenge = {
  id: number;
  title: string;
  difficulty: number;
  description: string;
  verified: boolean;
  username: string;
  viewerOwner: boolean;
};

export function ChallengeCard({challenge}:{challenge:Challenge}){
    console.log(challenge.verified);
    console.log(challenge.viewerOwner);
    return(
        <div>
            <p>{challenge.id}</p>
            <p>{challenge.title}</p>
            <p>{challenge.username}</p>
            {challenge.viewerOwner &&(
                <div>
                    <Link href={`/challenge/${challenge.id}/edit_general`}>Edit general</Link>
                    <Link href={`/challenge/${challenge.id}/languages`}>Languages support</Link>
                </div>
            )}
            <p>___</p>
        </div>
    )
}