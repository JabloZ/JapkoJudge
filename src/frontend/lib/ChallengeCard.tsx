export type Challenge = {
  id: number;
  title: string;
  difficulty: number;
  description: string;
  verified: boolean;
  username: string;
};

export function ChallengeCard({challenge}:{challenge:Challenge}){
    return(
        <div>
            <p>{challenge.id}</p>
            <p>{challenge.title}</p>
            <p>{challenge.username}</p>
        </div>
    )
}