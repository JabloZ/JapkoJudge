import { redirect } from "next/navigation";
import { getSession, isAuthor } from "@/lib/session";
import { AddLanguageForm } from "./AddLanguageForm";
export default async function AddLanguagePage({params}:{params:Promise<{id:string}>}) {
    //todo: check if viewer is author
    const session = await getSession();
    if (!session) {
        redirect("/");
    }
    const{id}=await params;
    const isUserAuthor=await isAuthor(Number.parseInt(id));
    if (!isUserAuthor){
        redirect(`/challenge/${id}`);
    }
    return <AddLanguageForm id={id}/>;
}